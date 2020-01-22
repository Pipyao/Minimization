using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{


	class Transition
	{
		public string From;
		public string Symb;
		public string To;
		public override string ToString()
		{
			return $"{From} {Symb} {To}";
		}
	}

	class Nedeterm
	{
		public string[] Alphabet;
		public string[] StateMachine;
		public string InitialStateMachine;
		public string[] FinalStateMachine;
		public HashSet<string> CurrentState = new HashSet<string>();// хэш сет
		public List<Transition> transitions = new List<Transition>();

		public void Validate()
		{
			if (Alphabet.Length == 0)
				throw new Exception($"Алфавит не может быть пустым");

			if (StateMachine.Length == 0)
				throw new Exception($"Количество состояний не может быть пустым");

			for (int i = 0; i < FinalStateMachine.Length; i++)
			{
				if (!(StateMachine.Contains(FinalStateMachine[i])))
					throw new Exception($"Финальные состояния не содержаться в таблице состояний ");
			}

			if (!(StateMachine.Contains(InitialStateMachine)))
				throw new Exception($"Начальное состояние не содержиться в таблице состояний ");

			for (int i = 0; i < transitions.Count; i++)
			{

				if (!StateMachine.Contains(transitions[i].From))
					throw new Exception($"Исходное состояние не содержиться в списке: {transitions[i]}");
				if (!StateMachine.Contains(transitions[i].To))
					throw new Exception($"Целевое состояние не содержиться в списке: {transitions[i]}");
				if (!(Alphabet.Contains(transitions[i].Symb)))
					throw new Exception($"Символ в переходах не содержиться в алфавите: {transitions[i]}");
			}
		}

		public void Mashine(string EnterString)
		{
			if (!Alphabet.Contains(EnterString))
				throw new Exception($"Введенный символ \"{EnterString}\" не содержиться в алфавите");

			bool flag = false;
			HashSet<string> TmpCurrent = new HashSet<string>();
			for (int j = 0; j < transitions.Count; j++)
			{
				if (CurrentState.Contains(transitions[j].From) && transitions[j].Symb == EnterString)
				{
					flag = true;
					TmpCurrent.Add(transitions[j].To);
				}
			}
			Console.WriteLine("\t" + string.Join(", ", TmpCurrent));
			EpsilonTransition(TmpCurrent);
			Console.WriteLine(string.Join(", ", TmpCurrent));
			CurrentState = TmpCurrent;
			if (!flag)
			{
				throw new Exception($"Нет соответствующего перехода, строка непринята");
			}
		}


		public void EpsilonTransition(HashSet<string> states)
		{
			HashSet<string> TmpStates = new HashSet<string>();
			for (int j = 0; j < transitions.Count; j++)
			{
				if (states.Contains(transitions[j].From) && transitions[j].Symb == "_")
				{
					TmpStates.Add(transitions[j].To);
				}
			}
			TmpStates.ExceptWith(states);
			if (TmpStates.Count != 0)
			{
				states.UnionWith(TmpStates);
				EpsilonTransition(states);
			}
		}

		public void Reset()
		{
			CurrentState.Clear();
			CurrentState.Add(InitialStateMachine);
		}

		public bool End()
		{
			return CurrentState.Intersect(FinalStateMachine).Any();
		}


		public Determ NewMashine()
		{
			Determ result = new Determ();

			List<List<string>> CompositeState = new List<List<string>>();
			HashSet<string> TmpStates = new HashSet<string>();

			result.Alphabet = Alphabet;
			result.InitialStateMachine = InitialStateMachine;
			result.StateMachine.Add(InitialStateMachine);
			var l = new List<string>();
			l.Add(InitialStateMachine);
			CompositeState.Add(l);

			for (int i = 0; i <= result.StateMachine.Count - 1; i++)
			{
				for (int j = 0; j < Alphabet.Length; j++)
				{
					if (Alphabet[j] != "_")
					{
						TmpStates.Clear();
						for (int k = 0; k < transitions.Count; k++)
						{
							if (CompositeState[i].Contains(transitions[k].From) && transitions[k].Symb == Alphabet[j])
							{
								TmpStates.Add(transitions[k].To);
							}
						}
						EpsilonTransition(TmpStates);
						List<string> names = new List<string>(TmpStates);
						names.Sort();
						if (names.Count > 0)
						{
							string statename = null;
							for (int t = 0; t < CompositeState.Count; t++)
							{
								if (names.SequenceEqual(CompositeState[t]))
								{
									statename = result.StateMachine[t];
									break;
								}
							}
							if (statename == null)
							{
								statename = string.Join("_", names);
								result.StateMachine.Add(statename);
								CompositeState.Add(names);
								TmpStates.IntersectWith(FinalStateMachine);
								if (TmpStates.Count > 0)
									result.FinalStateMachine.Add(statename);
							}
							DTransition tr = new DTransition();
							tr.From = result.StateMachine[i];
							tr.Symb = Alphabet[j];
							tr.To = statename;
							result.transitions.Add(tr);
						}
					}
				}	
			}
			return result;
		}


		public void Output()
		{
			Determ det = new Determ();
			det = NewMashine();

			string writePath = @"C:\temp\MyTest.txt";
		
			try
			{
				using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
				{

					sw.Write("Alphabet: ");

					for (int i = 0; i < det.Alphabet.Length; i++)
						sw.Write(det.Alphabet[i] + ", ");
					sw.Write("\n\n");

					sw.Write("StateMachine: ");
					for (int i = 0; i < det.StateMachine.Count; i++)
						sw.Write(det.StateMachine[i] + ", "); 
					sw.Write("\n\n");

					sw.Write("InitialStateMachine: ");
					for (int i = 0; i < det.InitialStateMachine.Length; i++)
						sw.Write(det.InitialStateMachine[i] + ", ");
					sw.Write("\n\n");

					sw.Write("FinalStateMachine: ");
					for (int i = 0; i < det.FinalStateMachine.Count; i++)
						sw.Write(det.FinalStateMachine[i] + ", ");
					sw.Write("\n\n");

					sw.Write("Transitions: ");
					sw.Write("\n\n");
					for (int i = 0; i < det.transitions.Count; i++)
						sw.Write(det.transitions[i].ToString() + "\n");
				}
				Console.WriteLine("Запись выполнена");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
			public void Input()
			{
				string path = @"Test 2.txt";
				try
				{

					using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
					{
						string buffer;
						string A, Q, Final_Q;

						A = sr.ReadLine();
						Alphabet = A.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

						Q = sr.ReadLine();
						StateMachine = Q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

						InitialStateMachine = sr.ReadLine();

						Final_Q = sr.ReadLine();

						FinalStateMachine = Final_Q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

						while ((buffer = sr.ReadLine()) != null)
						{
							string[] parts = buffer.Split(new char[] { ' ' }, 3);
							if (parts.Length == 3)
							{
								Transition transition = new Transition();
								transition.From = parts[0];
								transition.Symb = parts[1];
								transition.To = parts[2];

								transitions.Add(transition);
							}
							else
								throw new Exception($"Некорректный переход: {buffer}");
						}
					}
					Validate();
					
				}
				catch (Exception Ex)
				{
					Console.WriteLine(Ex.Message);
				}
			}

		public void Work()
		{
			try
			{
				Reset();
				Console.WriteLine("Введите строку разделяя символы пробелами");
				string tmp_string = "";
				tmp_string = Console.ReadLine();
				string[] input = tmp_string.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				for (int i = 0; i < input.Length; i++)
					Mashine(input[i]);

				if (End())
					Console.WriteLine("Автомат перешел в конечное состояние");
				else
					Console.WriteLine("Автомат не перешёл в конечное состояние");
				Reset();
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}




