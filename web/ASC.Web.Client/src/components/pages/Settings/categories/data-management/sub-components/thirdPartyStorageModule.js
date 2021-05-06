import React from "react";
import { withTranslation } from "react-i18next";

import Button from "@appserver/components/button";
import Text from "@appserver/components/text";

import { inject, observer } from "mobx-react";
import Box from "@appserver/components/box";
import Link from "@appserver/components/link";
import ComboBox from "@appserver/components/combobox";
import { getBackupStorage } from "@appserver/common/api/settings";
import styled from "styled-components";

const StyledComponent = styled.div`
  .manual-backup_text-input {
    margin-top: 16px;
  }
`;
class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      availableOptions: [],
      availableStorage: [],
      selectedOption: "",
      isLoading: false,
    };
  }
  componentDidMount() {
    this.setState(
      {
        isLoading: true,
      },
      function () {
        getBackupStorage()
          .then((storageBackup) => this.getOptions(storageBackup))
          .finally(() => this.setState({ isLoading: false }));
      }
    );
  }

  getOptions = (storageBackup) => {
    let options = [];
    let availableStorage = [];
    //debugger;
    for (let item = 0; item < storageBackup.length; item++) {
      let obj = {
        [storageBackup[item].title]: storageBackup[item].isSet,
      };
      let titleObj = {
        key: item,
        label: storageBackup[item].title,
        disabled: false,
      };
      options.push(titleObj);
      availableStorage.push(obj);
      if (storageBackup[item].id === "GoogleCloud") {
        this.setState({
          selectedOption: storageBackup[item].title,
        });
      }
    }
    this.setState({
      availableOptions: options,
      availableStorage: availableStorage,
    });
  };

  render() {
    const { maxProgress, t, helpUrlCreatingBackup, storageBackup } = this.props;
    const {
      availableOptions,
      availableStorage,
      selectedOption,
      isLoading,
    } = this.state;
    console.log("availableOptions", availableOptions);
    console.log("availableStorage", availableStorage);
    return (
      <StyledComponent>
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
          <ComboBox
            options={availableOptions}
            selectedOption={{ key: 0, label: selectedOption }}
            onSelect={() => ""}
            isDisabled={isLoading}
            noBorder={false}
            scaled={true}
            scaledOptions={true}
            dropDownMaxHeight={300}
            className="manual-backup_text-input"
          />
          <Box marginProp="16px 0 16px 0">
            <Link
              color="#316DAA"
              target="_blank"
              isHovered={true}
              href={helpUrlCreatingBackup}
            >
              {t("LearnMore")}
            </Link>
          </Box>
          <div className="manual-backup_buttons">
            <Button
              label={t("MakeCopy")}
              onClick={() => console.log("click")}
              primary
              isDisabled={!maxProgress || isLoading}
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
  const { helpUrlCreatingBackup } = auth.settingsStore;

  return {
    helpUrlCreatingBackup,
  };
})(withTranslation("Settings")(observer(ThirdPartyStorageModule)));
