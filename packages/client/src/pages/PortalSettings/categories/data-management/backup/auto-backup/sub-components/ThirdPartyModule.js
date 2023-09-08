import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { BackupStorageType } from "@docspace/common/constants";
import ScheduleComponent from "./ScheduleComponent";

import DirectThirdPartyConnection from "../../common-container/DirectThirdPartyConnection";

class ThirdPartyModule extends React.PureComponent {
  constructor(props) {
    super(props);
    const { setSelectedFolder, isResourcesDefault } = props;

    this.state = {
      isPanelVisible: false,
    };

    !isResourcesDefault && setSelectedFolder("");
  }

  onSelectFolder = (id) => {
    const { setSelectedFolder } = this.props;

    setSelectedFolder(`${id}`);
  };
  render() {
    const { isPanelVisible } = this.state;
    const {
      isError,
      isLoadingData,
      isReset,
      buttonSize,
      passedId,
      //commonThirdPartyList,
      isResourcesDefault,
      t,
      ...rest
    } = this.props;

    return (
      <>
        <div className="auto-backup_third-party-module">
          <DirectThirdPartyConnection
            t={t}
            onSelectFolder={this.onSelectFolder}
            isDisabled={isLoadingData}
            isPanelVisible={isPanelVisible}
            withoutInitPath={!isResourcesDefault}
            isError={isError}
            id={passedId}
            buttonSize={buttonSize}
          />
        </div>
        <ScheduleComponent isLoadingData={isLoadingData} {...rest} />
      </>
    );
  }
}
export default inject(({ backup }) => {
  const {
    setSelectedFolder,

    defaultStorageType,
    commonThirdPartyList,
    defaultFolderId,
  } = backup;

  const isResourcesDefault =
    defaultStorageType === `${BackupStorageType.ResourcesModuleType}`;
  const passedId = isResourcesDefault ? defaultFolderId : "";

  return {
    setSelectedFolder,
    passedId,
    commonThirdPartyList,
    isResourcesDefault,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyModule)));
