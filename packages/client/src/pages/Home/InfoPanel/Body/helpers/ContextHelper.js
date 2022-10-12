class ContextHelper {
  constructor(props) {
    this.t = props.t;
    this.selection = props.selection;
    this.getContextOptions = props.getContextOptions;
    this.getContextOptionActions = props.getContextOptionActions;

    if (this.selection) this.fixItemContextOptions();
  }

  fixItemContextOptions = () => {
    let options = this.selection.contextOptions;
    if (!options) options = this.getContextOptions(this.selection, false);

    const showInfoIndex = options.findIndex((i) => i === "show-info");
    const lastIndex = options.length - 1;
    const isLastIndex = showInfoIndex === lastIndex;

    if (!showInfoIndex && options[1].includes("separator"))
      options = options.slice(2);
    else if (isLastIndex && options[lastIndex - 1].includes("separator"))
      options = options.slice(0, -2);
    else options = options.filter((i) => i !== "show-info");

    this.selection.contextOptions = options;
  };

  getItemContextOptions = () => {
    return this.getContextOptionActions(this.selection, this.t);
  };
}

export default ContextHelper;
