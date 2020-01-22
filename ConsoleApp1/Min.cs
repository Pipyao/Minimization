using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApp1
{
	class Min
	{
		string[] Alf;
		List<string> Sost= new List<string>();
	string Nach;//начальное состояние
	string Current;
	List<string> Kon = new List<string>();//конечне состояния
	List<string[]> Per = new List<string[]>();

	List<string> WorkStr = new List<string>();


		List<List<string>> nextClasses = new List<List<string>>();
		List<List<string>> prevClasses = new List<List<string>>() ;

		string path = "Test 2.txt";

	public Min()
	{
		int c = 0;
		using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
		{
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				if (c == 0)
				{
					Alf = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);// входные сигналы
					AlfCheck();
				}
				if (c == 1)
				{
						string[] mas1 = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);// состояния
						for (int i = 0; i < mas1.Length; i++)
						{
							Sost.Add(mas1[i]);// конечные состояния
						}
						SostCheck();// не пусты-ли состояния						
				}
				if (c == 2)
				{
					Nach = line;// начальное состояние
					if (!NachCheck())// начальное состояние принадлежит одному из состояний
						throw new Exception("Неизвестное начальное состояние");
					Current = Nach;
				}
				if (c == 3)
				{
					string[] mas1 = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < mas1.Length; i++)
					{
						Kon.Add(mas1[i]);// конечные состояния
					}
					string f = KonCheck();
					if (f != "")// конечные состояния принадлежат состояниям
						throw new Exception("Неизвестное конечное состояние: " + f);
				}
				if (c >= 4)
				{
					string[] mas = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					Per.Add(mas);// правила
				}

				c++;
			}
			string[] fl = NeizvPer();
			if (fl.Length != 0)
			{
				string s = "";
				for (int i = 0; i < fl.Length; i++)
				{
					s += fl[i];
				}
				throw new Exception("Неизвестное правило: " + s);
			}
			fl = DublirPer();
			if (fl.Length != 0)
			{
				string s = "";
				for (int i = 0; i < fl.Length; i++)
				{
					s += fl[i];
				}
				throw new Exception("Найдено дублирующееся состояние: " + s);
			}
		}
			DeleteMusor();
			Divide();
			bool k = false;
			while (k==false)
			k=NewLevel();
			WriteInFile();
	}

	private void AlfCheck()
	{
		if (Alf.Length == 0)
			throw new Exception("Алфавит пуст");
	}

	private void SostCheck()
	{
		if (Sost.Count == 0)
			throw new Exception("Состояния пусты");
	}
	private bool NachCheck()
	{
		for (int i = 0; i < Sost.Count; i++)
		{
			if (Nach == Sost[i])
			{
				return true;
			}
		}
		return false;
	}
	private string KonCheck()
	{
		bool flag = false;
		string fl = "";
		foreach (string s in Kon)
		{
			for (int i = 0; i < Sost.Count; i++)
			{
				if (s == Sost[i])
				{
					flag = true;
					break;
				}
				flag = false;
			}
			if (flag == false)
				return s;
		}
		return fl;
	}
	private string[] NeizvPer()
	{
		bool flag = false;
		bool flag1 = false;
		string[] good = new string[0];
		foreach (string[] s in Per)// правила
		{
			flag = false;
			for (int i = 0; i < Alf.Length; i++)// алфавит
			{
				if (s[1] == Alf[i])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
				return s;
		}

		foreach (string[] s in Per)// правила
		{
			flag = false;
			flag1 = false;
			for (int j = 0; j < Sost.Count; j++)// состояния
			{
				if (s[0] == Sost[j])
				{
					flag = true;
				}
				if (s[2] == Sost[j])
				{
					flag1 = true;
				}
				if (flag && flag1)
					break;
			}
			if (!flag || !flag1)
				return s;
		}
		return good;
	}
	private string[] DublirPer()
	{
		string[] good = new string[0];
		int count = 0;
		foreach (string[] s in Per)
		{
			count = 0;
			foreach (string[] s1 in Per)
			{
				if (s.SequenceEqual(s1))
					count++;
				if (count == 2)
					return s1;
			}
		}
		return good;
	}
		public void DeleteMusor()
		{
			for (int i = 0; i < Kon.Count; i++)
			{
				if (Sost.Contains(Kon[i]))
					Sost.Remove(Kon[i]);
			}
		}

		public void Divide()
		{
			nextClasses.Add(Sost);
			nextClasses.Add(Kon);
			
		}

		public bool NewLevel()
		{
			prevClasses = new List<List<string>>(nextClasses);
			nextClasses.Clear();

			foreach (List<string> cls in prevClasses)
			{
				List<string> newcls = new List<string>();
				newcls.Add(cls[0]);
				nextClasses.Add(newcls);
				for (int i = 1; i < cls.Count; i++)
				{
					bool clsfound = false;
					foreach (List<string> nextcls in nextClasses)
					{
						if (Equivalent(cls[i], nextcls[0]))
						{
							clsfound = true;
							nextcls.Add(cls[i]);
							break;
						}
					}
					if (!clsfound)
					{
						newcls = new List<string>();
						newcls.Add(cls[i]);
						nextClasses.Add(newcls);
					}
				}
			}
			
			return nextClasses.Count == prevClasses.Count;
		}

		public bool Equivalent(string state1, string state2)
		{
			if (!PrevEq(state1, state2))
				return false;

			Dictionary<string, string> OneState=new Dictionary<string, string>();
			Dictionary<string, string> TwoState=new Dictionary<string, string>();

			for (int i = 0; i < Per.Count; i++)
			{
				if (state1 == Per[i][0])
				{
					OneState.Add(Per[i][1], Per[i][2]);
				}
				else {
					if (state2 == Per[i][0])
						TwoState.Add(Per[i][1], Per[i][2]);
				}
			}

			foreach(string a in Alf)
			{
				if (OneState.ContainsKey(a) != TwoState.ContainsKey(a))
					return false;
				else
				{
					if (OneState.ContainsKey(a) && TwoState.ContainsKey(a))
					{
						if (!PrevEq(OneState[a], TwoState[a]))
							return false;
					}
				}
			}
			return true;
		}
		public bool PrevEq(string state1, string state2)
		{
			foreach (List<string> s in prevClasses)
			{
				if (s.Contains(state1) && s.Contains(state2))
					return true;
			}
			return false;
		}

		public void WriteInFile()
		{
			string s="";
			foreach (string a in Alf)
			{
				s += a+" ";
			}
			s += "\r\n";
			foreach (List<string> rul in prevClasses)
			{
				for (int i=0;i<rul.Count;i++)
				{
					s += rul[i];
					if (i != rul.Count - 1)
						s += "-";
				}
				s += " ";
			}
			StartnFinal();
			s += "\r\n";
			s += Nach;
			s += "\r\n";
			foreach (string a in Kon)
			{
				s += a + " ";
			}
			s += "\r\n";

			HashSet<string> S=Combinate();

			foreach (string t in S)
			{
				s += t;
				s += "\r\n";
			}
			
			File.AppendAllText("TestFile.txt", s);
		}

		public void StartnFinal()
		{
			foreach (List<string> pc in prevClasses)
			{
				if (pc.Contains(Nach))
				{ Nach = "";
					for (int i = 0; i < pc.Count; i++)
					{
						Nach += pc[i];
						if ( i!= pc.Count - 1)
							Nach += "-";
					}
				}		
			}


			foreach (List<string> pc in prevClasses)
			{
				for (int j = 0; j < Kon.Count; j++)
				{
					if (pc.Contains(Kon[j]))
					{
						Kon[j] = "";
						for (int i = 0; i < pc.Count; i++)
						{
							Kon[j] += pc[i];
							if (i != pc.Count - 1)
								Kon[j] += "-";
						}
					}
				}
			}


		}


		public HashSet<string> Combinate()
		{
			HashSet<string> S = new HashSet<string>();
			foreach (List<string> cur in prevClasses)
			{
				if (cur.Count>1)
				{
					string k = "";
					for (int i = 0; i < cur.Count; i++)
					{
						k += cur[i];
						if (i != cur.Count - 1)
							k += "-";
					}
					for (int i = 0; i < Per.Count; i++)
					{
						foreach (string s in cur)
						{
							if (Per[i][0] == s)
								Per[i][0] = k;
						}
					}

				}
			}

			string y;

			foreach (string[] s in Per)
			{
				
				for (int i = 0; i < Per.Count; i++)
				{
					y = "";
					y += Per[i][0] + " " + Per[i][1] + " " + Per[i][2];
					S.Add(y);
				}
				
			}
			return S;
			
		}
	}
}

