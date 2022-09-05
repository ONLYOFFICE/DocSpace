class ContextHelper {
  constructor(props) {
    this.t = props.t;
    this.item = props.selectedItem;
    this.getContextOptions = props.getContextOptions;
    this.getContextOptionActions = props.getContextOptionActions;

    this.fixItemContextOptions();
  }

  fixItemContextOptions = () => {
    let newContextOptions = this.item.contextOptions;

    if (!newContextOptions)
      newContextOptions = this.getContextOptions(this.item, false);
    newContextOptions = newContextOptions.filter((co) => co !== "show-info");

    this.item.contextOptions = newContextOptions;
  };

  getItemContextOptions = () => {
    return this.getContextOptionActions(this.item, this.t);
  };
}

export default ContextHelper;
