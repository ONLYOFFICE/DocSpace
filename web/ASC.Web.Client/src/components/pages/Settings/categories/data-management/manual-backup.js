import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import commonSettingsStyles from "../../utils/commonSettingsStyles";
import styled from "styled-components";
import Button from "@appserver/components/button";
import Checkbox from "@appserver/components/checkbox";
import { inject, observer } from "mobx-react";
import { getThirdPartyList } from "@appserver/common/api/files";
import DocumentsModule from "./sub-components/documentsModule";
import FileInputWithFolderPath from "@appserver/components/file-input-with-folder-path";

import OperationsDialog from "files/OperationsDialog";

const StyledComponent = styled.div`
  ${commonSettingsStyles}
  .manual-backup_buttons {
    margin-top: 16px;
  }
  .backup-include_mail {
    margin-top: 16px;
    margin-bottom: 16ox;
  }
  .inherit-title-link {
    margin-bottom: 8px;
  }
  .note_description {
    margin-top: 8px;
  }
  .backup-folder_path {
    margin-top: 16px;
  }
`;
// const ManualBackup = () => {
//   const { t } = useTranslation("Settings");
//   const [isChecked, setIsChecked] = useState(false);
//   const [isIncludeMail, setIsIncludeMail] = useState(false);

// ;
//   return (

//   );
// };

class ManualBackup extends React.Component {
  constructor(props) {
    super(props);
    this.manualBackup = true;

    // this.state = {
    //   backupMailTemporaryStorage: false,
    //   backupMailDocuments: false,
    //   backupMailThirdParty: false,
    //   backupMailThirdPartyStorage: false,
    // };
    this.state = {
      isVisiblePanel: false,
    };
  }
  componentDidMount() {
    // getThirdPartyList().then((res) => console.log("res", res));
  }
  onClickCheckbox = (e) => {
    const name = e.target.name;
    let change = !this.state[name];
    this.setState({ [name]: change });
  };

  render() {
    const { t, providers, panelVisible, folderPath } = this.props;
    // const {
    //   backupMailTemporaryStorage,
    //   backupMailDocuments,
    //   backupMailThirdParty,
    //   backupMailThirdPartyStorage,
    // } = this.state;
    console.log("folderPath", folderPath);
    return (
      <StyledComponent>
        <div className="category-item-wrapper temporary-storage">
          <div className="category-item-heading">
            <Text className="inherit-title-link header">
              {t("TemporaryStorage")}
            </Text>
          </div>
          <Text className="category-item-description">
            {t("TemporaryStorageDescription")}
          </Text>

          {/* <div className="backup-include_mail">
            <Checkbox
              name={"backupMailTemporaryStorage"}
              isChecked={backupMailTemporaryStorage}
              label={t("IncludeMail")}
              onChange={this.onClickCheckbox}
            />
          </div> */}
          <div className="manual-backup_buttons">
            <Button
              label={t("MakeCopy")}
              onClick={() => console.log("click")}
              primary
              isDisabled={false}
              size="medium"
              tabIndex={10}
            />
            <Button
              label={t("DownloadBackup")}
              onClick={() => console.log("click")}
              isDisabled={false}
              size="medium"
              style={{ marginLeft: "8px" }}
              tabIndex={11}
            />
          </div>
        </div>

        <div className="category-item-wrapper">
          <div className="category-item-heading">
            <Text className="inherit-title-link header">
              {t("DocumentsModule")}
            </Text>
          </div>

          <Text className="category-item-description">
            {t("DocumentsModuleDescription")}
          </Text>

          {/* <div className="backup-include_mail">
            <Checkbox
              name={"backupMailDocuments"}
              isChecked={backupMailDocuments}
              label={t("IncludeMail")}
              onChange={onClickCheckbox}
            />
          </div> */}

          <FileInputWithFolderPath scale className="backup-folder_path" />
          {panelVisible && <OperationsDialog />}
          <div className="manual-backup_buttons">
            <Button
              label={t("MakeCopy")}
              onClick={() => console.log("click")}
              primary
              isDisabled={false}
              size="medium"
              tabIndex={10}
            />
          </div>
        </div>
        <div className="category-item-wrapper temporary-storage">
          <div className="category-item-heading">
            <Text className="inherit-title-link header">
              {t("ThirdPartyResource")}
            </Text>
          </div>
          <Text className="category-item-description">
            {t("ThirdPartyResourceDescription")}
          </Text>
          <Text className="category-item-description note_description">
            {t("ThirdPartyResourceNoteDescription")}
          </Text>

          {/* <div className="backup-include_mail">
            <Checkbox
              name={"backupMailThirdParty"}
              isChecked={backupMailThirdParty}
              label={t("IncludeMail")}
              onChange={this.onClickCheckbox}
            />
          </div> */}
          <div className="manual-backup_buttons">
            <Button
              label={t("MakeCopy")}
              onClick={() => console.log("click")}
              primary
              isDisabled={false}
              size="medium"
              tabIndex={10}
            />
          </div>
        </div>

        <div className="category-item-wrapper temporary-storage">
          <div className="category-item-heading">
            <Text className="inherit-title-link header">
              {t("ThirdPartyStorage")}
            </Text>
          </div>
          <Text className="category-item-description">
            {t("ThirdPartyStorageDescription")}
          </Text>
          <Text className="category-item-description note_description">
            {t("ThirdPartyStorageNoteDescription")}
          </Text>

          {/* <div className="backup-include_mail">
            <Checkbox
              name={"backupMailThirdPartyStorage"}
              isChecked={backupMailThirdPartyStorage}
              label={t("IncludeMail")}
              onChange={this.onClickCheckbox}
            />
          </div> */}
          <div className="manual-backup_buttons">
            <Button
              label={t("MakeCopy")}
              onClick={() => console.log("click")}
              primary
              isDisabled={false}
              size="medium"
              tabIndex={10}
            />
          </div>
        </div>
      </StyledComponent>
    );
  }
}

export default inject(({ auth }) => {
  const { setPanelVisible, panelVisible } = auth;
  const { folderPath } = auth.settingsStore;
  return {
    setPanelVisible,
    panelVisible,
    folderPath,
  };
})(withTranslation("Settings")(observer(ManualBackup)));
