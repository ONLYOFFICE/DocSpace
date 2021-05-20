import React from "react";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";
import Box from "@appserver/components/box";
import Link from "@appserver/components/link";
import ComboBox from "@appserver/components/combobox";
import { getBackupStorage } from "@appserver/common/api/settings";
import styled from "styled-components";
import TextInput from "@appserver/components/text-input";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import Button from "@appserver/components/button";
import { startBackup } from "@appserver/common/api/portal";

const StyledComponent = styled.div`
  .backup_combo {
    margin-top: 16px;
    width: 100%;
    max-width: 820px;
  }
  .backup_text-input {
    margin: 10px 0;
    width: 100%;
    max-width: 820px;
  }
`;

let googleStorageId = "GoogleCloud";
let inputValueArray;
class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);
    this.isSetDefaultIdStorage = false;
    inputValueArray = [];
    this.state = {
      availableOptions: [],
      availableStorage: {},
      selectedOption: "",
      selectedId: "",
      isLoading: false,
      isSetDefaultStorage: true,
      isSelectedOptionChanges: false,
      isError: false,
      input_1: "",
      input_2: "",
      input_3: "",
      input_4: "",
      input_5: "",
      input_6: "",
    };
    this.isFirstSet = false;
    this.firstSetId = "";
  }
  componentDidMount() {
    this.setState(
      {
        isLoading: true,
      },
      function () {
        getBackupStorage()
          .then((storageBackup) => this.getOptions(storageBackup))
          .finally(() => {
            this.setState({ isLoading: false });
          });
      }
    );
  }

  getOptions = (storageBackup) => {
    const {
      isManualBackup,
      onSetDisableOptions,
      isDisableOptions,
    } = this.props;
    this.setState({
      isLoading: true,
    });
    let options = [];
    let availableStorage = {};

    //debugger;
    for (let item = 0; item < storageBackup.length; item++) {
      // debugger;
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
      //debugger;
      console.log("availableStorage", availableStorage);

      if (!isManualBackup && storageBackup[item].current) {
        this.isSetDefaultIdStorage = true;
        this.setState({
          selectedOption: storageBackup[item].title,
          defaultSelectedOption: storageBackup[item].title,
          selectedId: storageBackup[item].id,
          defaultSelectedId: storageBackup[item].id,
        });
      }

      if (this.isSetDefaultIdStorage)
        onSetDisableOptions && onSetDisableOptions(true);

      if (!this.isSetDefaultIdStorage && isDisableOptions)
        onSetDisableOptions && onSetDisableOptions(false);

      if (!this.isFirstSet && storageBackup[item].isSet) {
        this.isFirstSet = true;
        this.firstSetId = storageBackup[item].id;
      }
    }

    if (!this.isSetDefaultIdStorage && !this.isFirstSet) {
      this.setState({
        selectedOption: availableStorage[googleStorageId].title,
        defaultSelectedOption: availableStorage[googleStorageId].title,
        selectedId: availableStorage[googleStorageId].id,
        defaultSelectedId: availableStorage[googleStorageId].id,
        isSetDefaultStorage: false,
      });
    }

    if (!this.isSetDefaultIdStorage && this.isFirstSet) {
      this.setState({
        selectedOption: availableStorage[this.firstSetId].title,
        defaultSelectedOption: availableStorage[this.firstSetId].title,
        selectedId: availableStorage[this.firstSetId].id,
        defaultSelectedId: availableStorage[this.firstSetId].id,
        isSetDefaultStorage: false,
      });
    }

    this.setState({
      availableOptions: options,
      availableStorage: availableStorage,
      isLoading: false,
    });
  };

  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { availableStorage, isSetDefaultStorage } = this.state;
    const { onSetDisableOptions } = this.props;
    //debugger;
    if (isSetDefaultStorage) onSetDisableOptions && onSetDisableOptions(false);

    this.setState({
      selectedOption: availableStorage[selectedStorageId].title,
      selectedId: availableStorage[selectedStorageId].id,
      isSelectedOptionChanges: true,
      isSetDefaultStorage: false,
      isError: false,
      input_1: "",
      input_2: "",
      input_3: "",
      input_4: "",
      input_5: "",
      input_6: "",
    });
  };

  onChange = (event) => {
    const { target } = event;
    const value = target.value;
    const name = target.name;
    //debugger;
    // console.log("value", value);
    // console.log("name", name);

    this.setState({ [name]: value, isSelectedOptionChanges: true });
  };

  isInvalidForm = () => {
    const { selectedId, input_1, input_2 } = this.state;

    if (!input_1) {
      this.setState({
        isError: true,
      });
      return false;
    }
    //debugger;
    switch (selectedId) {
      case "Selectel":
        if (!input_2) {
          this.setState({
            isError: true,
          });
          return false;
        }
        return true;
      case "Rackspace":
        for (let i = 2; i <= 3; i++) {
          if (!this.state[`input_${i}`]) {
            this.setState({
              isError: true,
            });
            return false;
          }
        }
        return true;
      case "S3":
        for (let i = 2; i <= 6; i++) {
          if (!this.state[`input_${i}`]) {
            this.setState({
              isError: true,
            });
            return false;
          }
        }
        return true;
      default:
        return false;
    }
  };
  fillInputValueArray = () => {
    const { selectedId, availableStorage } = this.state;
    let obj = {};
    const selectedStorage = availableStorage[selectedId];

    for (let i = 1; i <= 6; i++) {
      if (this.state[`input_${i}`]) {
        obj = {
          key: selectedStorage.properties[i - 1].title,
          value: this.state[`input_${i}`],
        };
        inputValueArray.push(obj);
      }
    }
  };
  onSaveSettings = () => {
    const { fillStorageFields } = this.props;
    const { selectedId } = this.state;

    if (!this.isInvalidForm()) return;

    this.fillInputValueArray();

    this.setState({
      isSelectedOptionChanges: false,
      isError: false,
    });
    fillStorageFields(selectedId, inputValueArray);
  };

  onCancelSettings = () => {
    const { defaultSelectedOption, defaultSelectedId } = this.state;
    const { onCancelModuleSettings, onSetDisableOptions } = this.props;

    if (this.isSetDefaultIdStorage) {
      this.setState({
        isSetDefaultStorage: true,
      });
      onSetDisableOptions && onSetDisableOptions(true);
    }

    this.setState({
      selectedOption: defaultSelectedOption,
      selectedId: defaultSelectedId,
      isSelectedOptionChanges: false,
      input_1: "",
      input_2: "",
      input_3: "",
      input_4: "",
      input_5: "",
      input_6: "",
    });

    onCancelModuleSettings();
  };

  onMakeCopy = () => {
    const { setInterval } = this.props;
    const { selectedId } = this.state;

    if (!this.isInvalidForm()) return;
    this.fillInputValueArray();

    let storageParams = [
      {
        key: "module",
        value: selectedId,
      },
    ];

    let obj = {};

    for (let i = 0; i < inputValueArray.length; i++) {
      obj = {
        key: inputValueArray[i].key,
        value: inputValueArray[i].value,
      };
      storageParams.push(obj);
    }

    startBackup("5", storageParams);
    setInterval();
  };
  render() {
    const {
      t,
      helpUrlCreatingBackup,
      isChanged,
      isLoadingData,
      isCopyingToLocal,
      isManualBackup,
      maxProgress,
    } = this.props;
    const {
      availableOptions,
      availableStorage,
      selectedOption,
      isLoading,
      selectedId,
      input_1,
      input_2,
      input_3,
      input_4,
      input_5,
      input_6,
      isSetDefaultStorage,
      isSelectedOptionChanges,
      isError,
    } = this.state;
    console.log("selectedOption", isLoadingData);
    return (
      <StyledComponent>
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
        <ComboBox
          options={availableOptions}
          selectedOption={{ key: 0, label: selectedOption }}
          onSelect={this.onSelect}
          isDisabled={isLoadingData || isLoading}
          noBorder={false}
          scaled={true}
          scaledOptions={true}
          dropDownMaxHeight={300}
          className="backup_combo"
        />

        {!isSetDefaultStorage && (
          <>
            <TextInput
              name="input_1"
              className="backup_text-input"
              scale={true}
              value={input_1}
              hasError={isError}
              onChange={this.onChange}
              isDisabled={
                isLoadingData ||
                isLoading ||
                (availableStorage[selectedId] &&
                  !availableStorage[selectedId].isSet)
              }
              placeholder={
                availableStorage[selectedId] &&
                availableStorage[selectedId].properties[0].title
              }
              tabIndex={1}
            />

            {selectedId !== "GoogleCloud" && (
              <>
                <TextInput
                  name="input_2"
                  className="backup_text-input"
                  scale={true}
                  value={input_2}
                  hasError={isError}
                  onChange={this.onChange}
                  isDisabled={
                    isLoadingData ||
                    isLoading ||
                    (availableStorage[selectedId] &&
                      !availableStorage[selectedId].isSet)
                  }
                  placeholder={
                    availableStorage[selectedId] &&
                    availableStorage[selectedId].properties[1].title
                  }
                  tabIndex={1}
                />

                {(selectedId === "Rackspace" || selectedId === "S3") && (
                  <TextInput
                    name="input_3"
                    className="backup_text-input"
                    scale={true}
                    value={input_3}
                    hasError={isError}
                    onChange={this.onChange}
                    isDisabled={
                      isLoadingData ||
                      isLoading ||
                      (availableStorage[selectedId] &&
                        !availableStorage[selectedId].isSet)
                    }
                    placeholder={
                      availableStorage[selectedId] &&
                      availableStorage[selectedId].properties[2].title
                    }
                    tabIndex={1}
                  />
                )}
                {selectedId === "S3" && (
                  <>
                    <TextInput
                      name="input_4"
                      className="backup_text-input"
                      scale={true}
                      value={input_4}
                      hasError={isError}
                      onChange={this.onChange}
                      isDisabled={
                        isLoadingData ||
                        isLoading ||
                        (availableStorage[selectedId] &&
                          !availableStorage[selectedId].isSet)
                      }
                      placeholder={
                        availableStorage[selectedId] &&
                        availableStorage[selectedId].properties[3].title
                      }
                      tabIndex={1}
                    />
                    <TextInput
                      name="input_5"
                      className="backup_text-input"
                      scale={true}
                      value={input_5}
                      hasError={isError}
                      onChange={this.onChange}
                      isDisabled={
                        isLoadingData ||
                        isLoading ||
                        (availableStorage[selectedId] &&
                          !availableStorage[selectedId].isSet)
                      }
                      placeholder={
                        availableStorage[selectedId] &&
                        availableStorage[selectedId].properties[4].title
                      }
                      tabIndex={1}
                    />
                    <TextInput
                      name="input_6"
                      className="backup_text-input"
                      scale={true}
                      value={input_6}
                      hasError={isError}
                      onChange={this.onChange}
                      isDisabled={
                        isLoadingData ||
                        isLoading ||
                        (availableStorage[selectedId] &&
                          !availableStorage[selectedId].isSet)
                      }
                      placeholder={
                        availableStorage[selectedId] &&
                        availableStorage[selectedId].properties[5].title
                      }
                      tabIndex={1}
                    />
                  </>
                )}
              </>
            )}
          </>
        )}
        {!isManualBackup ? (
          (isSelectedOptionChanges || isChanged) && (
            <SaveCancelButtons
              className="team-template_buttons"
              onSaveClick={this.onSaveSettings}
              onCancelClick={this.onCancelSettings}
              showReminder={false}
              reminderTest={t("YouHaveUnsavedChanges")}
              saveButtonLabel={t("SaveButton")}
              cancelButtonLabel={t("CancelButton")}
              isDisabled={isCopyingToLocal || isLoadingData || isLoading}
            />
          )
        ) : (
          <div className="manual-backup_buttons">
            <Button
              label={t("MakeCopy")}
              onClick={this.onMakeCopy}
              primary
              isDisabled={!maxProgress}
              size="medium"
              tabIndex={10}
            />
          </div>
        )}
      </StyledComponent>
    );
  }
}
ThirdPartyStorageModule.defaultProps = {
  isManualBackup: false,
};
export default inject(({ auth }) => {
  const { helpUrlCreatingBackup } = auth.settingsStore;

  return {
    helpUrlCreatingBackup,
  };
})(withTranslation("Settings")(observer(ThirdPartyStorageModule)));
