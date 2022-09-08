class ContextHelper {
  constructor(props) {
    this.t = props.t;
    this.selection = props.selection;
    this.getContextOptions = props.getContextOptions;
    this.getContextOptionActions = props.getContextOptionActions;

    this.fixItemContextOptions();
  }

  fixItemContextOptions = () => {
    let newContextOptions = this.selection.contextOptions;

    if (!newContextOptions)
      newContextOptions = this.getContextOptions(this.selection, false);
    newContextOptions = newContextOptions.filter((co) => co !== "show-info");

    this.selection.contextOptions = newContextOptions;
  };

  getItemContextOptions = () => {
    return this.getContextOptionActions(this.selection, this.t);
  };
}

export default ContextHelper;
