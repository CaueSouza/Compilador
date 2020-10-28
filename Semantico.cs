using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compilador.Constantes;

namespace Compilador
{
    class Semantico
    {
        private Stack stack = Stack.Instance;
        private int actualLevel = 0;

        public Semantico()
        {

        }

        public void increaseLevel()
        {
            actualLevel++;
        }

        public void decreaseLevel()
        {
            actualLevel--;
        }

        public void resetLevel()
        {
            actualLevel = 0;
        }

        public void insereTabela(string lexema, string nome, int rotulo)
        {
            stack.push(new Struct(lexema, nome, actualLevel, rotulo));
        }

        public bool pesquisaDuplicVarTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.nome.Equals(NOME_VARIAVEL) && actualItem.lexema.Equals(lexema) && actualItem.nivel == actualLevel)
                {
                    return true;
                }
            }

            return false;
        }

        public void colocaTipoTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if ((actualItem.nome.Equals(NOME_VARIAVEL) || actualItem.nome.Equals(NOME_FUNCAO)) && actualItem.tipo == null)
                {
                    actualItem.tipo = lexema;
                }
                else
                {
                    return;
                }
            }
        }

        public bool pesquisaDeclVarTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.nome.Equals(NOME_VARIAVEL) && actualItem.lexema.Equals(lexema) && actualItem.nivel == actualLevel)
                {
                    return true;
                }
            }

            return false;
        }

        public bool pesquisaDeclVarFuncTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if ((actualItem.nome.Equals(NOME_VARIAVEL) || actualItem.nome.Equals(NOME_FUNCAO)) && actualItem.lexema.Equals(lexema) && actualItem.nivel == actualLevel)
                {
                    return true;
                }
            }

            return false;
        }

        public bool pesquisaDeclProcTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.nome.Equals(NOME_PROCEDIMENTO) && actualItem.lexema.Equals(lexema) && actualItem.nivel == actualLevel)
                {
                    return true;
                }
            }

            return false;
        }

        public bool pesquisaDeclFuncTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.nome.Equals(NOME_FUNCAO) && actualItem.lexema.Equals(lexema) && actualItem.nivel == actualLevel)
                {
                    return true;
                }
            }

            return false;
        }

        public Struct pesquisaTabela(string lexema, int indice)
        {
            return null;
        }

        public void voltaNivel()
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.nivel == actualLevel)
                {
                    stack.pop();
                }
                else
                {
                    return;
                }
            }
        }
    }
}
