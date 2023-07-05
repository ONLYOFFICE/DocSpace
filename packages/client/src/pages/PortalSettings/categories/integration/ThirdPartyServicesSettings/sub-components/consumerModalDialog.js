import React from "react";
import PropTypes from "prop-types";
import { Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import { format } from "react-string-format";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import TextInput from "@docspace/components/text-input";
import Box from "@docspace/components/box";
import Link from "@docspace/components/link";
import toastr from "@docspace/components/toast/toastr";
import ModalDialogContainer from "./modalDialogContainer";
import { showLoader, hideLoader } from "@docspace/common/utils";
import { hugeMobile } from "@docspace/components/utils/device";
import styled from "styled-components";

const StyledBox = styled(Box)`
  padding: 20px 0 8px;
  @media ${hugeMobile} {
    padding-top: 0;
  }
`;

class ConsumerModalDialog extends React.Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  mapTokenNameToState = () => {
    const { selectedConsumer } = this.props;
    selectedConsumer.props.map((prop) =>
      this.setState({
        [`${prop.name}`]: prop.value,
      })
    );
  };

  onChangeHandler = (e) => {
    this.setState({
      [e.target.name]: e.target.value,
    });
  };

  updateConsumerValues = () => {
    const {
      onChangeLoading,
      selectedConsumer,
      updateConsumerProps,
      onModalClose,
      t,
    } = this.props;
    const { state } = this;

    onChangeLoading(true);
    showLoader();
    const prop = [];

    let i = 0;
    let stateLength = Object.keys(state).length;
    for (i = 0; i < stateLength; i++) {
      prop.push({
        name: Object.keys(state)[i],
        value: Object.values(state)[i],
      });
    }
    const data = {
      name: selectedConsumer.name,
      props: prop,
    };
    updateConsumerProps(data)
      .then(() => {
        onChangeLoading(false);
        hideLoader();
        toastr.success(t("ThirdPartyPropsActivated"));
      })
      .catch((error) => {
        onChangeLoading(false);
        hideLoader();
        toastr.error(error);
      })
      .finally(onModalClose());
  };

  // shouldComponentUpdate(nextProps, nextState) {
  //   console.log("this.state: ", this.state, "nextState: ", nextState);
  //   return nextState !== this.state;
  // }

  componentDidMount() {
    this.mapTokenNameToState();
  }

  thirdPartyServicesUrl = () => {
    switch (this.props.selectedConsumer.name) {
      case "docusign" || "docuSign":
        return this.props.docuSignUrl;
      case "dropbox":
        return this.props.dropboxUrl;
      case "box":
        return this.props.boxUrl;
      case "mailru":
        return this.props.mailRuUrl;
      case "skydrive":
        return this.props.oneDriveUrl;
      case "microsoft":
        return this.props.microsoftUrl;
      case "google":
        return this.props.googleUrl;
      case "facebook":
        return this.props.facebookUrl;
      case "linkedin":
        return this.props.linkedinUrl;
      case "clickatell":
        return this.props.clickatellUrl;
      case "smsc":
        return this.props.smsclUrl;
      case "firebase":
        return this.props.firebaseUrl;
      case "appleID":
        return this.props.appleIDUrl;
      case "telegram":
        return this.props.telegramUrl;
      case "wordpress":
        return this.props.wordpressUrl;
      case "s3":
        return this.props.awsUrl;
      case "googlecloud":
        return this.props.googleCloudUrl;
      case "rackspace":
        return this.props.rackspaceUrl;
      case "selectel":
        return this.props.selectelUrl;
      case "yandex":
        return this.props.yandexUrl;
      case "vk":
        return this.props.vkUrl;
      default:
        return this.props.docspaceSettingsUrl;
    }
  };

  consumerInstruction =
    this.props.selectedConsumer.instruction &&
    format(this.props.selectedConsumer.instruction, <Box marginProp="0" />);

  helpCenterDescription = (
    <Trans t={this.props.t} i18nKey="ThirdPartyBodyDescription" ns="Settings">
      Detailed instructions in our{" "}
      <Link
        id="help-center-link"
        color={this.props.theme.client.settings.integration.linkColor}
        isHovered={false}
        target="_blank"
        href={this.thirdPartyServicesUrl()}
      >
        Help Center
      </Link>
    </Trans>
  );

  supportTeamDescription = (
    <StyledBox>
      <Trans
        t={this.props.t}
        i18nKey="ThirdPartyBottomDescription"
        ns="Settings"
      >
        If you still have some questions on how to connect this service or need
        technical assistance, please feel free to contact our{" "}
        <Link
          id="support-team-link"
          color={this.props.theme.client.settings.integration.linkColor}
          isHovered={false}
          target="_blank"
          href={this.props.urlSupport}
        >
          Support Team
        </Link>
      </Trans>
    </StyledBox>
  );

  render() {
    const { selectedConsumer, onModalClose, dialogVisible, isLoading, t } =
      this.props;
    const {
      state,
      onChangeHandler,
      updateConsumerValues,
      consumerInstruction,
      helpCenterDescription,
      supportTeamDescription,
    } = this;

    return (
      <ModalDialogContainer
        visible={dialogVisible}
        onClose={onModalClose}
        displayType="aside"
      >
        <ModalDialog.Header>{selectedConsumer.title}</ModalDialog.Header>
        <ModalDialog.Body>
          <Box paddingProp="0 0 16px">{consumerInstruction}</Box>
          <React.Fragment>
            {selectedConsumer.props.map((prop, i) => (
              <React.Fragment key={prop.name}>
                <Box
                  displayProp="flex"
                  flexDirection="column"
                  marginProp={
                    selectedConsumer.props.length == i + 1 ? "0" : "0 0 16px 0"
                  }
                >
                  <Box marginProp="0 0 4px 0">
                    <Text isBold>{prop.title}:</Text>
                  </Box>
                  <Box>
                    <TextInput
                      scale
                      id={prop.name}
                      name={prop.name}
                      placeholder={prop.title}
                      isAutoFocussed={i === 0}
                      tabIndex={1}
                      value={Object.values(state)[i]}
                      isDisabled={isLoading}
                      onChange={onChangeHandler}
                    />
                  </Box>
                </Box>
              </React.Fragment>
            ))}
          </React.Fragment>
          <Text as="div">{supportTeamDescription}</Text>
          <Text as="div">{helpCenterDescription}</Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            primary
            size="normal"
            id="enable-button"
            label={isLoading ? t("Common:Sending") : t("Common:Enable")}
            isLoading={isLoading}
            isDisabled={isLoading}
            scale
            onClick={updateConsumerValues}
          />
          <Button
            size="normal"
            scale
            id="cancel-button"
            label={t("Common:CancelButton")}
            isLoading={isLoading}
            isDisabled={isLoading}
            onClick={onModalClose}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

ConsumerModalDialog.propTypes = {
  t: PropTypes.func.isRequired,
  i18n: PropTypes.object.isRequired,
  selectedConsumer: PropTypes.object,
  onModalClose: PropTypes.func.isRequired,
  dialogVisible: PropTypes.bool.isRequired,
  isLoading: PropTypes.bool.isRequired,
  onChangeLoading: PropTypes.func.isRequired,
  updateConsumerProps: PropTypes.func.isRequired,
  urlSupport: PropTypes.string,
};

export default inject(({ setup, auth }) => {
  const { settingsStore } = auth;
  const {
    theme,
    urlSupport,
    docspaceSettingsUrl,
    docuSignUrl,
    dropboxUrl,
    boxUrl,
    mailRuUrl,
    oneDriveUrl,
    microsoftUrl,
    googleUrl,
    facebookUrl,
    linkedinUrl,
    clickatellUrl,
    smsclUrl,
    firebaseUrl,
    appleIDUrl,
    telegramUrl,
    wordpressUrl,
    awsUrl,
    googleCloudUrl,
    rackspaceUrl,
    selectelUrl,
    yandexUrl,
    vkUrl,
  } = settingsStore;
  const { integration } = setup;
  const { selectedConsumer } = integration;

  return {
    theme,
    selectedConsumer,
    urlSupport,
    docspaceSettingsUrl,
    docuSignUrl,
    dropboxUrl,
    boxUrl,
    mailRuUrl,
    oneDriveUrl,
    microsoftUrl,
    googleUrl,
    facebookUrl,
    linkedinUrl,
    clickatellUrl,
    smsclUrl,
    firebaseUrl,
    appleIDUrl,
    telegramUrl,
    wordpressUrl,
    awsUrl,
    googleCloudUrl,
    rackspaceUrl,
    selectelUrl,
    yandexUrl,
    vkUrl,
  };
})(observer(ConsumerModalDialog));
