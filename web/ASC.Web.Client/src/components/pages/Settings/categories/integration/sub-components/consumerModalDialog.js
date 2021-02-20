import React from "react";
import PropTypes from "prop-types";
import {
  ModalDialog,
  Text,
  Button,
  TextInput,
  Box,
  Link,
  toastr,
} from "asc-web-components";
import ModalDialogContainer from "./modalDialogContainer";
import { Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import { format } from "react-string-format";

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
        toastr.success(t("ThirdPartyPropsActivated"));
      })
      .catch((error) => {
        onChangeLoading(false);
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

  consumerInstruction =
    this.props.selectedConsumer.instruction &&
    format(this.props.selectedConsumer.instruction, <Box marginProp="4px 0" />);

  bodyDescription = (
    <Box marginProp={`${this.consumerInstruction ? "44px" : 0} 0 16px 0`}>
      <Box marginProp="0 0 16px 0">
        <Text as="div" isBold fontSize="15px">
          {this.props.t("ThirdPartyHowItWorks")}
        </Text>
      </Box>
      <Text as="div">
        <Trans i18nKey="ThirdPartyBodyDescription" ns="Settings">
          For more detailed instructions about connecting this service, please
          refer to our{" "}
          <Link
            color="#316DAA"
            isHovered={false}
            target="_blank"
            href={`${this.props.urlAuthKeys}#${this.props.selectedConsumer.name}`}
          >
            Help Center
          </Link>{" "}
          that provides all the necessary information.
        </Trans>
      </Text>
    </Box>
  );

  bottomDescription = (
    <Trans i18nKey="ThirdPartyBottomDescription" ns="Settings">
      If you still have some questions on how to connect this service or need
      technical assistance, please feel free to contact our{" "}
      <Link
        color="#316DAA"
        isHovered={false}
        target="_blank"
        href={this.props.urlSupport}
      >
        Support Team
      </Link>
    </Trans>
  );

  render() {
    const {
      selectedConsumer,
      onModalClose,
      dialogVisible,
      isLoading,
      t,
    } = this.props;
    const {
      state,
      onChangeHandler,
      updateConsumerValues,
      consumerInstruction,
      bodyDescription,
      bottomDescription,
    } = this;

    return (
      <ModalDialogContainer>
        <ModalDialog visible={dialogVisible} onClose={onModalClose}>
          <ModalDialog.Header>{selectedConsumer.title}</ModalDialog.Header>
          <ModalDialog.Body>
            <Text as="div">{consumerInstruction}</Text>
            <Text as="div">{bodyDescription}</Text>
            <React.Fragment>
              {selectedConsumer.props.map((prop, i) => (
                <React.Fragment key={prop.name}>
                  <Box
                    displayProp="flex"
                    flexDirection="column"
                    marginProp="0 0 16px 0"
                  >
                    <Box marginProp="0 0 4px 0">
                      <Text isBold>{prop.title}:</Text>
                    </Box>
                    <Box>
                      <TextInput
                        scale
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
            <Text>{bottomDescription}</Text>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              className="modal-dialog-button"
              primary
              size="big"
              label={
                isLoading
                  ? t("ThirdPartyProcessSending")
                  : t("ThirdPartyEnableButton")
              }
              tabIndex={1}
              isLoading={isLoading}
              isDisabled={isLoading}
              onClick={updateConsumerValues}
            />
          </ModalDialog.Footer>
        </ModalDialog>
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
  const { urlAuthKeys, urlSupport } = settingsStore;
  const { integration } = setup;
  const { selectedConsumer } = integration;

  return {
    selectedConsumer,
    urlSupport,
    urlAuthKeys,
  };
})(observer(ConsumerModalDialog));
