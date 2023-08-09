const excludeGeneralOptions = ["select", "show-info"];
const excludeRoomOptions = ["separator0", "room-info"];
const excludeOptionsIntoRoom = ["pin-room", "unpin-room"];
const excludeOptionsIntoFolder = ["open", "separator0", "separator1"];

class ContextHelper {
  constructor(props) {
    this.t = props.t;
    this.isUser = props.isUser;
    this.selection = props.selection;
    this.getContextOptions = props.getContextOptions;
    this.getUserContextOptions = props.getUserContextOptions;
    this.getContextOptionActions = props.getContextOptionActions;

    if (this.selection) this.fixItemContextOptions();
  }

  fixItemContextOptions = () => {
    if (this.isUser) {
      if (!this.selection?.options) return;
      const newOptions = this.selection.options.filter(
        (option) => option !== "details"
      );
      this.selection.options = newOptions;
      return;
    }

    const options = this.getContextOptions(this.selection, true);

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

    if (this.selection?.isSelectedFolder && !this.selection?.isRoom) {
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

  getItemContextOptions = () => {
    return this.isUser
      ? this.getUserContextOptions(
          this.t,
          this.selection.options || [],
          this.selection
        )
      : this.getContextOptionActions(
          this.selection,
          this.t,
          this.selection.isSelectedFolder
        );
  };
}

export default ContextHelper;
