using System;
using System.Text;
using System.Collections.Generic;
using Suru.Common.EncryptionLibrary;

namespace Suru.InsertGenerator.ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test_Asymetric_Encryption_Class();
            //Test_Symetric_Encryption_Class();
            //Test_Salt_and_Pass_Generator();
            Test_Ordering_Table_List();

            Console.WriteLine();
            Console.WriteLine("Press ENTER key to close.");
            Console.ReadLine();
        }

        //Test Symetric encryption / decryption features
        private static void Test_Symetric_Encryption_Class()
        {
            //El método soporta más de 255 caracteres...
            String TextToEncrypt = "123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345";

            Console.WriteLine("Text to process: " + TextToEncrypt);

            String EncryptedText = Encryption.SymetricEncrypt(TextToEncrypt);

            Console.WriteLine("Symetric Encrypted text: " + EncryptedText);

            Console.WriteLine("Symetric Decrypted text: " + Encryption.SymetricDecrypt(EncryptedText));
        }

        //Test Asymetric encryption / decryption features
        private static void Test_Asymetric_Encryption_Class()
        {
            String TextToEncrypt = "PassPassPass";

            Console.WriteLine("Text to process: " + TextToEncrypt);

            String EncryptedText = Encryption.AsymetricEncrypt(TextToEncrypt);

            Console.WriteLine("Asymetric Encrypted text: " + EncryptedText);

            Console.WriteLine("Asymetric Decrypted text: " + Encryption.AsymetricDecrypt(EncryptedText));
        }

        private static void Test_Salt_and_Pass_Generator()
        {
            Encryption.ResetCryptoKeys();
        }

        private static void Test_Ordering_Table_List()
        {
            //Number of tables to process
            Int16 NumberOfTables = 10;

            #region Local Variables

            //This matrix will tell dependency count and dependency graph
            Int16[,] TableOrder = new Int16[NumberOfTables, NumberOfTables];
            Int16[] ReferenceCountArray = new Int16[NumberOfTables];
            Int16[] RowOrderArray = new Int16[NumberOfTables];

            String tablename = null;
            Int16 ReferenceCount;
            Boolean IncrementI = true;

            #endregion

            #region Generate Dependancy Table Graph and PreOrdering arrays

            //Load ReferenceCountArray
            //Load RowOrderArray
            //Load TableOrder

            //Matrix:

            //      t1  t2  t3  t4  t5
            //  t1  0   1   0   0   0
            //  t2  0   0   1   1   0
            //  t3  1   0   0   0   0
            //  t4  0   0   0   0   1
            //  t5  0   0   0   0   0
            //
            // Order: t5, t4, t1 (or t1, t4), t3, t2

            // Original version result: t5(ok), t4(ok), t3(bad), t1(bad), t2 (ok)

            /*
            ReferenceCountArray[0] = 1;
            ReferenceCountArray[1] = 2;
            ReferenceCountArray[2] = 1;
            ReferenceCountArray[3] = 1;
            ReferenceCountArray[4] = 0;
             */

            ReferenceCountArray[0] = 5;
            ReferenceCountArray[1] = 3;
            ReferenceCountArray[2] = 1;
            ReferenceCountArray[3] = 1;
            ReferenceCountArray[4] = 4;
            ReferenceCountArray[5] = 0;
            ReferenceCountArray[6] = 2;
            ReferenceCountArray[7] = 2;
            ReferenceCountArray[8] = 1;
            ReferenceCountArray[9] = 1;

            RowOrderArray[0] = 0;
            RowOrderArray[1] = 1;
            RowOrderArray[2] = 2;
            RowOrderArray[3] = 3;
            RowOrderArray[4] = 4;
            RowOrderArray[5] = 5;
            RowOrderArray[6] = 6;
            RowOrderArray[7] = 7;
            RowOrderArray[8] = 8;
            RowOrderArray[9] = 9;

            /* 
            TableOrder[0, 0] = 0;
            TableOrder[0, 1] = 1;
            TableOrder[0, 2] = 0;
            TableOrder[0, 3] = 0;
            TableOrder[0, 4] = 0;
            TableOrder[1, 0] = 0;
            TableOrder[1, 1] = 0;
            TableOrder[1, 2] = 1;
            TableOrder[1, 3] = 1;
            TableOrder[1, 4] = 0;
            TableOrder[2, 0] = 1;
            TableOrder[2, 1] = 0;
            TableOrder[2, 2] = 0;
            TableOrder[2, 3] = 0;
            TableOrder[2, 4] = 0;
            TableOrder[3, 0] = 0;
            TableOrder[3, 1] = 0;
            TableOrder[3, 2] = 0;
            TableOrder[3, 3] = 0;
            TableOrder[3, 4] = 1;
            TableOrder[4, 0] = 0;
            TableOrder[4, 1] = 0;
            TableOrder[4, 2] = 0;
            TableOrder[4, 3] = 0;
            TableOrder[4, 4] = 0;
            */

            TableOrder[0, 0] = 0; TableOrder[0, 1] = 1; TableOrder[0, 2] = 1; TableOrder[0, 3] = 1; TableOrder[0, 4] = 1; TableOrder[0, 5] = 1; TableOrder[0, 6] = 0; TableOrder[0, 7] = 0; TableOrder[0, 8] = 0; TableOrder[0, 9] = 0;
            TableOrder[1, 0] = 0; TableOrder[1, 1] = 0; TableOrder[1, 2] = 1; TableOrder[1, 3] = 1; TableOrder[1, 4] = 0; TableOrder[1, 5] = 0; TableOrder[1, 6] = 0; TableOrder[1, 7] = 0; TableOrder[1, 8] = 0; TableOrder[1, 9] = 1;
            TableOrder[2, 0] = 0; TableOrder[2, 1] = 0; TableOrder[2, 2] = 0; TableOrder[2, 3] = 0; TableOrder[2, 4] = 0; TableOrder[2, 5] = 0; TableOrder[2, 6] = 0; TableOrder[2, 7] = 0; TableOrder[2, 8] = 0; TableOrder[2, 9] = 1;
            TableOrder[3, 0] = 0; TableOrder[3, 1] = 0; TableOrder[3, 2] = 1; TableOrder[3, 3] = 0; TableOrder[3, 4] = 0; TableOrder[3, 5] = 0; TableOrder[3, 6] = 0; TableOrder[3, 7] = 0; TableOrder[3, 8] = 0; TableOrder[3, 9] = 0;
            TableOrder[4, 0] = 1; TableOrder[4, 1] = 1; TableOrder[4, 2] = 1; TableOrder[4, 3] = 1; TableOrder[4, 4] = 0; TableOrder[4, 5] = 0; TableOrder[4, 6] = 0; TableOrder[4, 7] = 0; TableOrder[4, 8] = 0; TableOrder[4, 9] = 0;
            TableOrder[5, 0] = 0; TableOrder[5, 1] = 0; TableOrder[5, 2] = 0; TableOrder[5, 3] = 0; TableOrder[5, 4] = 0; TableOrder[5, 5] = 0; TableOrder[5, 6] = 0; TableOrder[5, 7] = 0; TableOrder[5, 8] = 0; TableOrder[5, 9] = 0;
            TableOrder[6, 0] = 1; TableOrder[6, 1] = 1; TableOrder[6, 2] = 0; TableOrder[6, 3] = 0; TableOrder[6, 4] = 0; TableOrder[6, 5] = 0; TableOrder[6, 6] = 0; TableOrder[6, 7] = 0; TableOrder[6, 8] = 0; TableOrder[6, 9] = 0;
            TableOrder[7, 0] = 0; TableOrder[7, 1] = 0; TableOrder[7, 2] = 0; TableOrder[7, 3] = 0; TableOrder[7, 4] = 0; TableOrder[7, 5] = 0; TableOrder[7, 6] = 0; TableOrder[7, 7] = 0; TableOrder[7, 8] = 1; TableOrder[7, 9] = 1;
            TableOrder[8, 0] = 1; TableOrder[8, 1] = 0; TableOrder[8, 2] = 0; TableOrder[8, 3] = 0; TableOrder[8, 4] = 0; TableOrder[8, 5] = 0; TableOrder[8, 6] = 0; TableOrder[8, 7] = 0; TableOrder[8, 8] = 0; TableOrder[8, 9] = 0;
            TableOrder[9, 0] = 0; TableOrder[9, 1] = 0; TableOrder[9, 2] = 0; TableOrder[9, 3] = 0; TableOrder[9, 4] = 0; TableOrder[9, 5] = 1; TableOrder[9, 6] = 0; TableOrder[9, 7] = 0; TableOrder[9, 8] = 0; TableOrder[9, 9] = 0;

            
            List<String> lTables = new List<String>();

            lTables.Add("t1");
            lTables.Add("t2");
            lTables.Add("t3");
            lTables.Add("t4");
            lTables.Add("t5");
            lTables.Add("t6");
            lTables.Add("t7");
            lTables.Add("t8");
            lTables.Add("t9");
            lTables.Add("t10");

            #endregion

            #region Ordering Arrays and Solving conflicts

            //RowOrder must be sorted by Reference Count. Less reference count, less dependancy.
            //Equal Reference Count checked by reference between tied ones.
            Array.Sort(ReferenceCountArray, RowOrderArray);

            //--> Row Order Array now contains the Ordered sequence of values by Reference Count.

            List<String> OrderedList = new List<String>();            

            //Order the list.
            Int16 i = 0, j = 0;

            //This procedure is a mess... If someone find a simple way email it to me --> fabrosell@gmail.com
            //The biggest problem is solving ties on reference count...
            while (i < NumberOfTables)
            {
                IncrementI = true;

                //Check for same dependency level
                if (i < NumberOfTables - 1)
                {
                    //More than one with same dependancy. Problem!
                    if (ReferenceCountArray[i] == ReferenceCountArray[i + 1])
                    {
                        #region Code to Solve a Tie

                        //Get the last position which has the same Reference Count
                        j =  i;

//Modificate test for equality to get right j index.
                        while (ReferenceCountArray[j] == ReferenceCountArray[j + 1] && j < NumberOfTables - 1)
                            j++;

                        Int16[] SubGroupReference = new Int16[j - i + 1];
                        Int16[] SubGroupOrder = new Int16[j - i + 1];

                        //Interval of same Reference Counts starts on i and ends on j.
                        //k goes by the reference counts
                        for (Int16 k = i; k <= j; k++)
                        {
                            SubGroupReference[k - i] = 0;
                            SubGroupOrder[k - i] = k;

                            for (Int16 z = i; z <= j; z++)
                            {
                                SubGroupReference[k - i] += TableOrder[RowOrderArray[k], RowOrderArray[z]];
                            }
                        }

                        //Ordering arrays
                        Array.Sort(SubGroupReference, SubGroupOrder);

                        //Now we have solved the conflict and we have the right dependancy order.
                        for (Int16 y = 0; y < SubGroupOrder.Length; y++)
                        {
                            OrderedList.Add(lTables[RowOrderArray[SubGroupOrder[y]]]);
                            i++;
                        }

                        IncrementI = false;

                        #endregion
                    }
                    else
                        OrderedList.Add(lTables[RowOrderArray[i]]);
                }
//Added next two sentences
                else
                    OrderedList.Add(lTables[RowOrderArray[i]]);

                if (IncrementI)
                    i++;
            }

            #endregion

            foreach (String s in OrderedList)
                Console.WriteLine(s);
        }

    }
}
