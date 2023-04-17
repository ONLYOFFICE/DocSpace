import React from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
import RoomSelector from "../../components/RoomSelector";
import SelectFolderDialog from "../../components/panels/SelectFolderDialog";

const Selector = ({ theme }) => {
  return (
    <SelectFolderDialog
      isPanelVisible={true}
      displayType="modal"
      header={null}
    />
  );
};

export default inject(({ auth }) => {
  const { theme } = auth.settingsStore;
  return {
    theme,
  };
})(observer(Selector));
