namespace JeuDeLaVie
{
    public class StructureTemplateNature : StructureTemplate
    {
        public double percentageChance;
        public StructureTemplateNature(byte[,] map, string id, double percentageChance) : base(map, id)
        {
            this.percentageChance = percentageChance;
        }
        public StructureTemplateNature(StructureTemplate st, double percentageChance) : base(st.StructureMap, st.Id)
        {
            this.percentageChance = percentageChance;
        }
    }
}
