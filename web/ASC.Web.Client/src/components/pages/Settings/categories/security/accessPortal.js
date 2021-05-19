import React, { Component } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Text from "@appserver/components/text";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import Button from "@appserver/components/button";
import toastr from "@appserver/components/toast/toastr";
import Loader from "@appserver/components/loader";
import { showLoader, hideLoader } from "@appserver/common/utils";

import { setDocumentTitle } from "../../../../../helpers/utils";
import { inject } from "mobx-react";

const MainContainer = styled.div`
  width: 100%;

  .save-button {
    margin-top: 32px;
  }

  .page_loader {
    position: fixed;
    left: 50%;
  }
`;

const HeaderContainer = styled.div`
  margin: 0 0 16px 0;
`;

class PureAccessPortal extends Component {
  constructor(props) {
    super(props);

    const { t } = props;
    this.state = { isLoaded: false, type: "none", showButton: false };

    setDocumentTitle(t("PortalSecurity"));
  }

  async componentDidMount() {
    const { getTfaSettings } = this.props;
    showLoader();

    await getTfaSettings().then((type) => {
      this.setState({ type: type, isLoaded: true });
      hideLoader();
    });
  }

  onSelectTfaType = async (e) => {
    const { getTfaSettings } = this.props;

    const type = await getTfaSettings();

    if (type !== e.target.value) {
      this.setState({ type: e.target.value, showButton: true });
    } else {
      this.setState({ type: e.target.value, showButton: false });
    }
  };

  saveSettings = () => {
    const { type } = this.state;
    const { t, setTfaSettings, getTfaConfirmLink, history } = this.props;

    setTfaSettings(type).then((res) => {
      toastr.success(t("SuccessfullySaveSettingsMessage"));
      if (type !== "none") {
        getTfaConfirmLink(type, res).then((link) =>
          history.push(link.replace(window.location.origin, ""))
        );
      }
      this.setState({ type: type, showButton: false });
    });
  };

  render() {
    const { isLoaded, type, showButton } = this.state;
    const { t } = this.props;

    return !isLoaded ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <MainContainer>
        <HeaderContainer>
          <Text fontSize="18px">{t("TwoFactorAuth")}</Text>
        </HeaderContainer>
        <RadioButtonGroup
          fontSize="13px"
          fontWeight="400"
          name="group"
          orientation="vertical"
          options={[
            {
              label: t("Disabled"),
              value: "none",
            },
            {
              label: t("BySms"),
              value: "sms",
              disabled: true, //TODO: get from api, when added sms tfa
            },
            {
              label: t("ByApp"),
              value: "app",
            },
          ]}
          selected={type}
          onClick={this.onSelectTfaType}
        />
        {showButton && (
          <Button
            label={t("SaveButton")}
            size="medium"
            primary={true}
            className="save-button"
            onClick={this.saveSettings}
          />
        )}
      </MainContainer>
    );
  }
}

export default inject(({ auth }) => ({
  organizationName: auth.settingsStore.organizationName,
  getTfaSettings: auth.tfaStore.getTfaSettings,
  setTfaSettings: auth.tfaStore.setTfaSettings,
  getTfaConfirmLink: auth.tfaStore.getTfaConfirmLink,
}))(withTranslation("Settings")(withRouter(PureAccessPortal)));
