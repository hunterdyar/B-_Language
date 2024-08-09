namespace BMinus.Compiler;

public class UnknownGoTo
{
    public InstructionLocation GotoLocation;
    public string LabelName;

    public UnknownGoTo(string labelName, InstructionLocation gotoLocation)
    {
        this.LabelName = labelName;
        this.GotoLocation = gotoLocation;
        
        Compiler.OnInstructionRemoved += OnInstructionRemoved;
    }

    public void TryFindLabel(Compiler compiler)
    {
        compiler.Labels.TryGetValue(LabelName, out var loc);
        //todo: Update the Frame. We need a way to get the frame from the labels dictionary?
    }
    private void OnInstructionRemoved(int frameID, int instIndex)
    {
        GotoLocation.OnOtherInstructionRemoved(frameID, instIndex);
    }
}