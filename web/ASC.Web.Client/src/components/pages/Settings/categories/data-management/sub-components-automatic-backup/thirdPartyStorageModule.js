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

const StyledComponent = styled.div`
  .manual-backup_combo {
    margin-top: 16px;
  }
  .backup_text-input {
    margin: 10px 0;
  }
`;

let googleStorageId = "GoogleCloud";

class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);
    this.isSetDefaultIdStorage = false;
    this.state = {
      availableOptions: [],
      availableStorage: {},
      selectedOption: "",
      selectedId: "",
      isLoading: false,
      isSetDefaultStorage: true,
      isSelectedOptionChanges: false,
      isError: false,
      firstInput: "",
      secondInput: "",
      thirdInput: "",
      fourthInput: "",
      fifthInput: "",
      sixthInput: "",
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
      //debugger;

      if (storageBackup[item].current) {
        this.isSetDefaultIdStorage = true;
        this.setState({
          selectedOption: storageBackup[item].title,
          defaultSelectedOption: storageBackup[item].title,
          selectedId: storageBackup[item].id,
          defaultSelectedId: storageBackup[item].id,
        });
      }
    }

    if (!this.isSetDefaultIdStorage) {
      this.setState({
        selectedOption: availableStorage[googleStorageId].title,
        defaultSelectedOption: availableStorage[googleStorageId].title,
        selectedId: availableStorage[googleStorageId].id,
        defaultSelectedId: availableStorage[googleStorageId].id,
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
    const { availableStorage } = this.state;

    this.setState({
      selectedOption: availableStorage[selectedStorageId].title,
      selectedId: availableStorage[selectedStorageId].id,
      isSelectedOptionChanges: true,
      isSetDefaultStorage: false,
      isError: false,
      firstInput: "",
      secondInput: "",
      thirdInput: "",
      fourthInput: "",
      fifthInput: "",
      sixthInput: "",
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
    const {
      selectedId,
      firstInput,
      secondInput,
      thirdInput,
      fifthInput,
      fourthInput,
      sixthInput,
    } = this.state;

    if (!firstInput) {
      this.setState({
        isError: true,
      });
      return false;
    }

    if (selectedId !== googleStorageId) {
      if (!secondInput) {
        this.setState({
          isError: true,
        });
        return false;
      }
      if (selectedId === "Rackspace") {
        if (!thirdInput) {
          this.setState({
            isError: true,
          });
          return false;
        }
      }

      if (selectedId === "S3") {
        if (!fifthInput) {
          this.setState({
            isError: true,
          });
          return false;
        }
        if (!fourthInput) {
          this.setState({
            isError: true,
          });
          return;
        }
        if (!sixthInput) {
          this.setState({
            isError: true,
          });
          return false;
        }
      }
    }
    return true;
  };
  onSaveSettings = () => {
    const { fillStorageFields } = this.props;
    const {
      selectedId,
      firstInput,
      secondInput,
      thirdInput,
      fifthInput,
      fourthInput,
      sixthInput,
      availableStorage,
    } = this.state;

    if (!this.isInvalidForm()) return;

    let obj = {};
    const selectedStorage = availableStorage[selectedId];
    const inputValueArray = [
      { key: selectedStorage.properties[0].title, value: firstInput },
    ];
    //debugger;
    if (secondInput) {
      obj = {
        key: selectedStorage.properties[1].title,
        value: secondInput,
      };
      inputValueArray.push(obj);
    }
    if (thirdInput) {
      obj = {
        key: selectedStorage.properties[2].title,
        value: thirdInput,
      };
      inputValueArray.push(obj);
    }

    if (fourthInput) {
      inputValueArray.push(
        { key: selectedStorage.properties[3].title, value: fourthInput },
        { key: selectedStorage.properties[4].title, value: fifthInput },
        { key: selectedStorage.properties[5].title, value: sixthInput }
      );
    }
    this.setState({
      isSelectedOptionChanges: false,
      isError: false,
    });

    fillStorageFields(selectedId, inputValueArray);
  };

  onCancelSettings = () => {
    const { defaultSelectedOption, defaultSelectedId } = this.state;
    const { onCancelModuleSettings } = this.props;
    this.isSetDefaultIdStorage &&
      this.setState({
        isSetDefaultStorage: true,
      });

    this.setState({
      selectedOption: defaultSelectedOption,
      selectedId: defaultSelectedId,
      isSelectedOptionChanges: false,
      firstInput: "",
      secondInput: "",
      thirdInput: "",
      fourthInput: "",
      fifthInput: "",
      sixthInput: "",
    });

    onCancelModuleSettings();
  };

  render() {
    const { t, helpUrlCreatingBackup, isChanged } = this.props;
    const {
      availableOptions,
      availableStorage,
      selectedOption,
      isLoading,
      selectedId,
      firstInput,
      secondInput,
      thirdInput,
      fourthInput,
      fifthInput,
      sixthInput,
      isSetDefaultStorage,
      isSelectedOptionChanges,
      isError,
    } = this.state;

    return isLoading ? (
      <></>
    ) : (
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
          isDisabled={isLoading}
          noBorder={false}
          scaled={true}
          scaledOptions={true}
          dropDownMaxHeight={300}
          className="manual-backup_combo"
        />

        {!isSetDefaultStorage && (
          <>
            <TextInput
              name="firstInput"
              className="backup_text-input"
              scale={true}
              value={firstInput}
              hasError={isError}
              onChange={this.onChange}
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

            {selectedId !== "GoogleCloud" && (
              <>
                <TextInput
                  name="secondInput"
                  className="backup_text-input"
                  scale={true}
                  value={secondInput}
                  hasError={isError}
                  onChange={this.onChange}
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

                {(selectedId === "Rackspace" || selectedId === "S3") && (
                  <TextInput
                    name="thirdInput"
                    className="backup_text-input"
                    scale={true}
                    value={thirdInput}
                    hasError={isError}
                    onChange={this.onChange}
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
                )}
                {selectedId === "S3" && (
                  <>
                    <TextInput
                      name="fourthInput"
                      className="backup_text-input"
                      scale={true}
                      value={fourthInput}
                      hasError={isError}
                      onChange={this.onChange}
                      isDisabled={
                        availableStorage[selectedId] &&
                        !availableStorage[selectedId].isSet
                      }
                      placeholder={
                        availableStorage[selectedId] &&
                        availableStorage[selectedId].properties[3].title
                      }
                      tabIndex={1}
                    />
                    <TextInput
                      name="fifthInput"
                      className="backup_text-input"
                      scale={true}
                      value={fifthInput}
                      hasError={isError}
                      onChange={this.onChange}
                      isDisabled={
                        availableStorage[selectedId] &&
                        !availableStorage[selectedId].isSet
                      }
                      placeholder={
                        availableStorage[selectedId] &&
                        availableStorage[selectedId].properties[4].title
                      }
                      tabIndex={1}
                    />
                    <TextInput
                      name="sixthInput"
                      className="backup_text-input"
                      scale={true}
                      value={sixthInput}
                      hasError={isError}
                      onChange={this.onChange}
                      isDisabled={
                        availableStorage[selectedId] &&
                        !availableStorage[selectedId].isSet
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
        {(isSelectedOptionChanges || isChanged) && (
          <SaveCancelButtons
            className="team-template_buttons"
            onSaveClick={this.onSaveSettings}
            onCancelClick={this.onCancelSettings}
            showReminder={false}
            reminderTest={t("YouHaveUnsavedChanges")}
            saveButtonLabel={t("SaveButton")}
            cancelButtonLabel={t("CancelButton")}
            isDisabled={this.props.isCopyingToLocal || this.props.isLoadingData}
          />
        )}
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
