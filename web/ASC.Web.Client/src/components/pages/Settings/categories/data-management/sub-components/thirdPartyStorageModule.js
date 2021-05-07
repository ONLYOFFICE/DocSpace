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
import TextInput from "@appserver/components/text-input";

const StyledComponent = styled.div`
  .manual-backup_combo {
    margin-top: 16px;
  }
  .backup_text-input {
    margin: 10px 0;
  }
`;
class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      availableOptions: [],
      availableStorage: {},
      selectedOption: "",
      selectedId: "",
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
    this.setState({
      isLoading: true,
    });
    let options = [];
    let availableStorage = {};
    //debugger;
    for (let item = 0; item < storageBackup.length; item++) {
      let obj = {
        [storageBackup[item].id]: {
          isSet: storageBackup[item].isSet,
          properties: storageBackup[item].properties,
          title: storageBackup[item].title,
          id: storageBackup[item].id,
        },
      };
      let titleObj = {
        key: storageBackup[item].id,
        label: storageBackup[item].title,
        disabled: false,
      };
      options.push(titleObj);

      availableStorage = { ...availableStorage, ...obj };
      if (storageBackup[item].id === "GoogleCloud") {
        this.setState({
          selectedOption: storageBackup[item].title,
          selectedId: storageBackup[item].id,
        });
      }
    }
    this.setState({
      availableOptions: options,
      availableStorage: availableStorage,
      isLoading: false,
    });
  };

  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { availableStorage } = this.state;
    this.setState({
      selectedOption: availableStorage[selectedStorageId].title,
      selectedId: availableStorage[selectedStorageId].id,
    });
  };
  render() {
    const { maxProgress, t, helpUrlCreatingBackup, storageBackup } = this.props;
    const {
      availableOptions,
      availableStorage,
      selectedOption,
      isLoading,
      selectedId,
    } = this.state;

    return isLoading ? (
      <></>
    ) : (
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
            onSelect={this.onSelect}
            isDisabled={isLoading}
            noBorder={false}
            scaled={true}
            scaledOptions={true}
            dropDownMaxHeight={300}
            className="manual-backup_combo"
          />

          {selectedId === "GoogleCloud" && (
            <TextInput
              name={"GoogleCloud_bucket"}
              className="backup_text-input"
              scale={true}
              //value={"value"}
              //hasError={formErrors.userCaption}
              onChange={() => console.log("onChange")}
              isDisabled={
                availableStorage[selectedId] &&
                !availableStorage[selectedId].isSet
              }
              placeholder={
                availableStorage[selectedId] &&
                availableStorage[selectedId].properties[0].title
              }
              tabIndex={1}
            />
          )}

          {selectedId === "Selectel" && (
            <>
              <TextInput
                name={"Selectel_private_container"}
                className="backup_text-input"
                scale={true}
                //value={"value"}
                //hasError={formErrors.userCaption}
                onChange={() => console.log("onChange")}
                isDisabled={
                  availableStorage[selectedId] &&
                  !availableStorage[selectedId].isSet
                }
                placeholder={
                  availableStorage[selectedId] &&
                  availableStorage[selectedId].properties[0].title
                }
                tabIndex={1}
              />
              <TextInput
                name={"Selectel_public_container"}
                className="backup_text-input"
                scale={true}
                //value={"value"}
                //hasError={formErrors.userCaption}
                onChange={() => console.log("onChange")}
                isDisabled={
                  availableStorage[selectedId] &&
                  !availableStorage[selectedId].isSet
                }
                placeholder={
                  availableStorage[selectedId] &&
                  availableStorage[selectedId].properties[1].title
                }
                tabIndex={1}
              />
            </>
          )}

          {selectedId === "Rackspace" && (
            <>
              <TextInput
                name={"Rackspace_private_container"}
                className="backup_text-input"
                scale={true}
                //value={"value"}
                //hasError={formErrors.userCaption}
                onChange={() => console.log("onChange")}
                isDisabled={
                  availableStorage[selectedId] &&
                  !availableStorage[selectedId].isSet
                }
                placeholder={
                  availableStorage[selectedId] &&
                  availableStorage[selectedId].properties[0].title
                }
                tabIndex={1}
              />
              <TextInput
                name={"Rackspace_public_container"}
                className="backup_text-input"
                scale={true}
                //value={"value"}
                //hasError={formErrors.userCaption}
                onChange={() => console.log("onChange")}
                isDisabled={
                  availableStorage[selectedId] &&
                  !availableStorage[selectedId].isSet
                }
                placeholder={
                  availableStorage[selectedId] &&
                  availableStorage[selectedId].properties[1].title
                }
                tabIndex={1}
              />
              <TextInput
                name={"Rackspace_region"}
                className="backup_text-input"
                scale={true}
                //value={"value"}
                //hasError={formErrors.userCaption}
                onChange={() => console.log("onChange")}
                isDisabled={
                  availableStorage[selectedId] &&
                  !availableStorage[selectedId].isSet
                }
                placeholder={
                  availableStorage[selectedId] &&
                  availableStorage[selectedId].properties[2].title
                }
                tabIndex={1}
              />
            </>
          )}
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
