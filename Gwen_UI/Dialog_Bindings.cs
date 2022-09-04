
using COE5_Map_Editor.Core.Input;
using COE5_Map_Editor.Core.Operations;
using Gwen.Net;
using Gwen.Net.Control;
using Gwen.Net.Xml;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace COE5_Map_Editor;

public class Dialog_Bindings : Gwen.Net.Control.Window
{
    public Action<string>? Callback { get; set; }

    private bool m_OnClosing;
    private PropertyTree m_Binding_List;

    private Popup m_Keybind_Popup;

    private readonly 
        Dictionary
        <
            Operation_Info,
            List<Input_Binding>
        > 
        m_Binding_Record =
        new Dictionary<Operation_Info, List<Input_Binding>>();

    private readonly Dictionary<Properties, Operation_Info>
        m_Property_To_Operation_Table =
        new Dictionary<Properties, Operation_Info>();

    public delegate bool Validate__Binding_Request
    (Input_Binding requested, Operation_Info operation_info);
    public delegate bool? Validate__Binding_Transition
    (Input_Binding old_binding, Input_Binding new_binding, Operation_Info target);

    public Dialog_Bindings
    (
        ControlBase parent, 
        IEnumerable<KeyValuePair<Input_Binding, Operation_Info>> bindings,
        //TODO:
        // reminder on how to impl:
        // if true, proceed, if false
        // abort and notify user the binding
        // to the intended operation is deemed
        // illegal. Ex. Move_To__Plane cannot bind
        // to a non-numeric binding.
        Validate__Binding_Request validate__binding_request,
        //TODO:
        // just a reminder on how to do this
        // see if op being unbinded is chill with
        // having no bind. Then if not, see if its
        // shiesty with swapping. If not, notify the
        // user why the binding failed and the op
        // responsible. (true, false, null) 
        //              might be best to encapsulate here.
        Validate__Binding_Transition validate__binding_transition
    ) 
    : base(parent)
    {
        foreach(KeyValuePair<Input_Binding, Operation_Info> pair in bindings) 
        {
            if (m_Binding_Record.ContainsKey(pair.Value))
            {
                m_Binding_Record[pair.Value].Add(pair.Key);
                continue;
            }

            m_Binding_Record.Add(pair.Value, new List<Input_Binding>() { pair.Key });
            //Properties prop = m_Binding_List.Add(pair.Value.Type_Name);
            //prop.Add(pair.Key.);
            //m_Property_To_Operation_Table.Add(prop, pair.Value);
        }
    }
    
    protected override bool OnChar(char chr)
    {
        if (!m_Is_Keybinding)
            return false;
        
        return true;
    }

    bool m_Is_Keybinding;
    private void Handle__Begin_Keybinding(ControlBase sender, ClickedEventArgs args)
    {

    }

    private void Handle__Finish_Keybinding()
    {

    }

    private void OnOkClicked(ControlBase sender, ClickedEventArgs args)
    {
        OnClosing(true);
    }
                             
    private void OnCancelClicked(ControlBase sender, ClickedEventArgs args)
    {
        OnClosing(true);
    }
                             
    private void OnWindowClosed(ControlBase sender, EventArgs args)
    {
        OnClosing(false);
    }

    protected virtual void OnClosing(bool doClose)
    {
        if (m_OnClosing)
            return;

        m_OnClosing = true;
    }
}
