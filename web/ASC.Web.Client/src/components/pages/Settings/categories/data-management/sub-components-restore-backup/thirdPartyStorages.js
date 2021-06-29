import React from "react";
import PropTypes from "prop-types";

import FileInput from "@appserver/components/file-input";
import { getOptions } from "../utils/getOptions";
import { getBackupStorage } from "../../../../../../../../../packages/asc-web-common/api/settings";
import ComboBox from "@appserver/components/combobox";

class ThirdPartyStorages extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      availableOptions: [],
      availableStorage: "",

      selectedStorage: "",
      selectedId: "",
      isLoading: false,
    };
  }
  componentDidMount() {
    this.setState({ isLoading: true }, function () {
      getBackupStorage()
        .then((storageBackup) => {
          const parameters = getOptions(storageBackup);

          const {
            options,
            availableStorage,
            selectedStorage,
            selectedId,
          } = parameters;

          this.setState({
            availableOptions: options,
            availableStorage: availableStorage,

            selectedStorage: selectedStorage,
            selectedId: selectedId,
          });
        })
        .finally(() =>
          this.setState({
            isLoading: false,
          })
        );
    });
  }
  render() {
    const { availableOptions, selectedStorage, isLoading } = this.state;

    return (
      <>
        <ComboBox
          options={availableOptions}
          selectedOption={{ key: 0, label: selectedStorage }}
          onSelect={() => console.log("console")}
          isDisabled={isLoading}
          noBorder={false}
          scaled={true}
          scaledOptions={true}
          dropDownMaxHeight={400}
          className="backup_combo"
        />
        <FileInput scale className="restore-backup_input" />
      </>
    );
  }
}

export default ThirdPartyStorages;
