import React from "react";
import { withTranslation } from "react-i18next";
import SelectFolderInput from "files/SelectFolderInput";
import ScheduleComponent from "./ScheduleComponent";

class ThirdPartyModule extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      isPanelVisible: false,
    };
  }

  onClickInput = () => {
    this.setState({
      isPanelVisible: true,
    });
  };

  onClose = () => {
    this.setState({
      isPanelVisible: false,
    });
  };

  render() {
    const { isPanelVisible } = this.state;
    const {
      onSelectFolder,
      isError,
      isLoadingData,
      isReset,
      isThirdPartyDefault,
      defaultSelectedFolder,
      isSuccessSave,
      ...rest
    } = this.props;

    const passedId = isThirdPartyDefault ? defaultSelectedFolder : "";
    return (
      <>
        <div className="auto-backup_folder-input">
          <SelectFolderInput
            onSelectFolder={onSelectFolder}
            onClose={this.onClose}
            onClickInput={this.onClickInput}
            isPanelVisible={isPanelVisible}
            isError={isError}
            foldersType="third-party"
            isSavingProcess={isLoadingData}
            id={passedId}
            isReset={isReset}
            isSuccessSave={isSuccessSave}
          />
        </div>
        <ScheduleComponent isLoadingData={isLoadingData} {...rest} />
      </>
    );
  }
}
export default withTranslation("Settings")(ThirdPartyModule);
