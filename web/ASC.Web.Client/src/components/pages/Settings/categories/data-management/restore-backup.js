import React from "react";

import { withTranslation } from "react-i18next";

import Button from "@appserver/components/button";
import Loader from "@appserver/components/loader";
import Checkbox from "@appserver/components/checkbox";
import Text from "@appserver/components/text";

import { StyledComponent } from "./styled-backup";
class RestoreBackup extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isChecked: false,
      isNotify: true,
    };
  }

  onChangeCheckbox = () => {
    this.setState({
      isChecked: !this.state.isChecked,
    });
  };
  onChangeCheckboxNotify = () => {
    this.setState({
      isNotify: !this.state.isNotify,
    });
  };
  render() {
    const { t } = this.props;
    const { isChecked, isLoading, isNotify } = this.state;

    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledComponent>
        <Text className="category-item-description">
          {t("DataRestoreDescription")}
        </Text>
        <Text className="category-item-description restore-source">
          {t("Source")}
        </Text>

        <Checkbox
          truncate
          className="restore-backup-checkbox_notification"
          onChange={this.onChangeCheckboxNotify}
          isChecked={isNotify}
          label={t("RestoreNotification")}
        />

        <Text className="category-item-description restore-source restore-warning">
          {t("Common:Warning")}
          {"!"}
        </Text>
        <Text className="category-item-description ">
          {t("RestoreSettingsWarning")}
        </Text>
        <Text className="category-item-description restore-warning_link">
          {t("RestoreSettingsLink")}
        </Text>

        <Checkbox
          truncate
          className="restore-backup-checkbox"
          onChange={this.onChangeCheckbox}
          isChecked={isChecked}
          label={t("RestoreAgreement")}
        />
        <Button
          label={t("RestoreButton")}
          onClick={() => console.log("click")}
          primary
          isDisabled={!isChecked}
          size="medium"
          tabIndex={10}
        />
      </StyledComponent>
    );
  }
}

export default withTranslation(["Settings", "Common"])(RestoreBackup);
