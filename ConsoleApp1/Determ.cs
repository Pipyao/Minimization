using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	class Determ
	{
		public string[] Alphabet;
		public List<string> StateMachine = new List<string>();
		public string InitialStateMachine;
		public List<string> FinalStateMachine = new List<string>();
		public string CurrentState;
		public List<DTransition> transitions = new List<DTransition>();

		public void Validate()
		{
			if (Alphabet.Length == 0)
				throw new Exception($"Алфавит не может быть пустым");

			if (StateMachine.Count == 0)
				throw new Exception($"Количество состояний не может быть пустым");

			for (int i = 0; i < FinalStateMachine.Count; i++)
			{
				if (!(StateMachine.Contains(FinalStateMachine[i])))
					throw new Exception($"Финальные состояния не содержаться в таблице состояний ");
			}

			if (!(StateMachine.Contains(InitialStateMachine)))
				throw new Exception($"Начальное состояние не содержиться в таблице состояний ");

			for (int i = 0; i < transitions.Count; i++)
			{
				if (!(Alphabet.Contains(transitions[i].Symb)))
					throw new Exception($"Символ в переходах не содержиться в алфавите");
			}

			for (int i = 0; i < transitions.Count; i++)
			{
				for (int j = i + 1; j < transitions.Count; j++)
				{
					if ((transitions[i].From == transitions[j].From) && (transitions[i].Symb == transitions[j].Symb))
						throw new Exception($"Имеются одинаковые переходы");
				}
			}


		}

		public void Mashine(string[] EnterString)
		{
			CurrentState = InitialStateMachine;
			for (int i = 0; i < EnterString.Length; i++)
			{
				for (int j = 0; j < transitions.Count; j++)
				{
					if (transitions[j].From == CurrentState && transitions[j].Symb == EnterString[i])
					{
						CurrentState = transitions[j].To;
						Console.WriteLine("Состояние: " + CurrentState);
						break;
					}
				}
			}
			if (FinalStateMachine.Contains(CurrentState))
				Console.WriteLine("Автомат перешел в конечное состояние");
			else
				Console.WriteLine("Автомат не перешел в конечное состояние");

		}

		public bool ValudateInput(string input)
		{
			if (Alphabet.Contains(input))
				return true;
			else
				return false;
		}
		public void Reset()
		{
			CurrentState = InitialStateMachine;
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

					String[] TmpQ = Q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

					for (int i = 0; i < TmpQ.Length; i++)
						StateMachine.Add(TmpQ[i]);

					InitialStateMachine = sr.ReadLine();

					Final_Q = sr.ReadLine();
					String[] TmpF_Q = Final_Q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

					for (int i = 0; i < TmpF_Q.Length; i++)
						StateMachine.Add(TmpF_Q[i]);

					while ((buffer = sr.ReadLine()) != null)
					{
						string[] parts = buffer.Split(new char[] { ' ' }, 3);
						if (parts.Length == 3)
						{
							DTransition transition = new DTransition();
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
				Reset();
				Console.WriteLine("Введите строку разделяя символы пробелами");
				string tmp_string = "";
				tmp_string = Console.ReadLine();
				string[] input = tmp_string.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				Mashine(input);

				Reset();
			}
			catch (Exception Ex)
			{
				Console.WriteLine(Ex.Message);
			}
			Console.ReadKey();
		}


	}

	class DTransition
	{
		public string From;
		public string Symb;
		public string To;

		public override string ToString()
		{
			return $"{From} {Symb} {To}";
		}
	}
}
