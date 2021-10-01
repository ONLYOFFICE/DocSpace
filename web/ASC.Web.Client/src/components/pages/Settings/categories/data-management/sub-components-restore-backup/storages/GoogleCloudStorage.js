import React from "react";
import { withTranslation } from "react-i18next";

import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";

class GoogleCloudStorage extends React.Component {
  constructor(props) {
    super(props);
    const { availableStorage, selectedId, onSetFormNames } = this.props;

    this.state = {
      bucket: "",
      path: "",
    };

    onSetFormNames(["bucket", "path"]);

    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;

    this.bucketPlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[0].title;
  }

  componentWillUnmount() {
    this.props.onResetFormSettings();
  }

  render() {
    const { t, isLoading, formSettings, onChange, isErrors } = this.props;
    console.log("isErrors", isErrors);
    return (
      <>
        <TextInput
          name="bucket"
          className="backup_text-input"
          scale={true}
          value={formSettings.bucket}
          onChange={onChange}
          isDisabled={isLoading || this.isDisabled}
          placeholder={this.bucketPlaceholder}
          tabIndex={1}
          hasError={isErrors.bucket ? true : false}
        />
        <TextInput
          name="path"
          className="backup_text-input"
          scale={true}
          value={formSettings.path}
          onChange={onChange}
          isDisabled={isLoading || this.isDisabled}
          placeholder={t("Path")}
          tabIndex={2}
          hasError={isErrors.path ? true : false}
        />
      </>
    );
  }
}
export default withTranslation("Settings")(GoogleCloudStorage);
