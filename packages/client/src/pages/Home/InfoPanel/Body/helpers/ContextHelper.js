const excludeGeneralOptions = ["select", "show-info"];
const excludeRoomOptions = ["separator0", "room-info"];
const excludeOptionsIntoRoom = ["pin-room", "unpin-room"];
const excludeOptionsIntoFolder = ["open", "separator0", "separator1"];

class ContextHelper {
  constructor(props) {
    this.t = props.t;
    this.selection = props.selection;
    this.setSelection = props.setSelection;
    this.reloadSelection = props.reloadSelection;
    this.getContextOptions = props.getContextOptions;
    this.getContextOptionActions = props.getContextOptionActions;

    if (this.selection) this.fixItemContextOptions();
  }

  fixItemContextOptions = () => {
    const options = this.getContextOptions(this.selection, false);

    excludeGeneralOptions.forEach((excludeOption) => {
      const idx = options.findIndex((o) => o === excludeOption);
      if (idx !== -1) options.splice(idx, 1);
    });

    if (this.selection?.isRoom) {
      excludeRoomOptions.forEach((excludeOption) => {
        const idx = options.findIndex((o) => o === excludeOption);
        if (idx !== -1) options.splice(idx, 1);
      });

      if (this.selection.isSelectedFolder) {
        excludeOptionsIntoRoom.forEach((excludeOption) => {
          const idx = options.findIndex((o) => o === excludeOption);
          if (idx !== -1) options.splice(idx, 1);
        });
      }
    }

    if (this.selection?.isSelectedFolder) {
      excludeOptionsIntoFolder.forEach((excludeOption) => {
        const idx = options.findIndex((o) => o === excludeOption);
        if (idx !== -1) options.splice(idx, 1);
      });
    }

    const length = options.length;

    options.forEach((item, index) => {
      if (
        (index !== length - 1 &&
          item.includes("separator") &&
          options[index + 1].includes("separator")) ||
        (index === 0 && item.includes("separator")) ||
        (index === length - 1 && item.includes("separator"))
      ) {
        options.splice(index, 1);
      }
    });

    // const showInfoIndex = options.findIndex((i) => i === "show-info");
    // const lastIndex = options.length - 1;
    // const isLastIndex = showInfoIndex === lastIndex;

    // if (!showInfoIndex && options[1].includes("separator"))
    //   options = options.slice(2);
    // else if (isLastIndex && options[lastIndex - 1].includes("separator"))
    //   options = options.slice(0, -2);
    // else options = options.filter((i) => i !== "show-info");

    this.selection.contextOptions = options;
  };

  fixItemContextOptionActions = () => {
    const contextOptionActions = this.getContextOptionActions(
      this.selection,
      this.t
    );

    const newContextOptionActions = contextOptionActions.map((coa) => ({
      ...coa,
      onClick: async (props) => {
        coa.onClick && (await coa.onClick(props));
        this.reloadSelection();
      },
    }));

    return newContextOptionActions;
  };

  getItemContextOptions = () => {
    // return this.fixItemContextOptionActions();
    return this.getContextOptionActions(this.selection, this.t);
  };
}

export default ContextHelper;
