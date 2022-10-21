const excludeRoomOptions = ["select", "separator0", "room-info"];
const excludeOptionsIntoRoom = ["pin-room", "unpin-room"];

class ContextHelper {
  constructor(props) {
    this.t = props.t;
    this.selection = { ...props.selection };
    this.getContextOptions = props.getContextOptions;
    this.getContextOptionActions = props.getContextOptionActions;

    this.selectedFolderId = props.selectedFolderId;

    if (this.selection) this.fixItemContextOptions();
  }

  fixItemContextOptions = () => {
    const options = this.getContextOptions(this.selection, false);

    if (this.selection?.isRoom) {
      excludeRoomOptions.forEach((excludeOption) => {
        const idx = options.findIndex((o) => o === excludeOption);

        if (idx !== -1) options.splice(idx, 1);
      });

      if (this.selection.id === this.selectedFolderId) {
        excludeOptionsIntoRoom.forEach((excludeOption) => {
          const idx = options.findIndex((o) => o === excludeOption);

          if (idx !== -1) options.splice(idx, 1);
        });
      }
    }

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

  getItemContextOptions = () => {
    return this.getContextOptionActions(this.selection, this.t);
  };
}

export default ContextHelper;
