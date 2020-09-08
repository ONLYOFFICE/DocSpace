import React from "react";
import { connect } from "react-redux";
import {
  getConsumers,
  sendConsumerNewProps,
} from "../../../../../store/settings/actions";
import { withTranslation } from "react-i18next";
import styled from "styled-components";

import { Box, Text, Link } from "asc-web-components";
import { utils } from "asc-web-components";
import ConsumerItem from "./sub-components/consumerItem";
import ConsumerModalDialog from "./sub-components/consumerModalDialog";

const tablet = utils.device.tablet;
const mobile = utils.device.mobile;

const RootContainer = styled(Box)`
  @media ${tablet} {
    margin: 0;

    .consumers-list-container {
      margin: 32px 0 40px 0;
    }
  }

  @media ${tablet} {
    .consumer-item-wrapper {
      margin: 0 0 24px 0;
    }
  }
`;
const StyledConsumer = styled(Box)`
  width: 400px;

  @media ${tablet} {
    width: 496px;
  }

  @media ${mobile} {
    width: 343px;
  }
`;
const Separator = styled.div`
  border: 1px solid #eceef1;
`;

class ThirdPartyServices extends React.Component {
  constructor(props) {
    super(props);
    const { t } = props;
    document.title = `${t("ThirdPartyAuthorization")} – ${t(
      "OrganizationName"
    )}`;

    this.state = {
      selectedConsumer: "",
      dialogVisible: false,
      isLoading: false,
    };
  }

  componentDidMount() {
    const { getConsumers } = this.props;
    getConsumers();
  }

  onChangeLoading = (status) => {
    this.setState({
      isLoading: status,
    });
  };

  onModalOpen = () => {
    this.setState({
      dialogVisible: true,
    });
  };

  onModalClose = () => {
    this.setState({
      dialogVisible: false,
      selectedConsumer: "",
    });
  };

  onModalButtonClick = () => {
    //TODO: api -> set tokens,
    //this.onModalClose();
    //this.setState({ toggleActive: true });
    console.log(this.state.selectedConsumer);
  };

  // onToggleClick = () => {
  //     this.onModalOpen();
  // }

  setConsumer = (e) => {
    this.setState({
      selectedConsumer: e.currentTarget.dataset.consumer,
    });
  };

  render() {
    const { t, consumers, sendConsumerNewProps } = this.props;
    const { selectedConsumer, dialogVisible, isLoading } = this.state;
    const {
      onModalClose,
      onModalOpen,
      onToggleClick,
      setConsumer,
      onModalButtonClick,
      onChangeLoading,
    } = this;

    return (
      <>
        <RootContainer
          displayProp="flex"
          flexDirection="column"
          marginProp="0 88px 0 0"
        >
          <Box className="title-description-container">
            <Text>{t("ThirdPartyTitleDescription")}</Text>
            <Box marginProp="16px 0 0 0">
              <Link
                color="#316DAA"
                isHovered={false}
                target="_blank"
                href="https://helpcenter.onlyoffice.com/ru/server/windows/community/authorization-keys.aspx"
              >
                {t("LearnMore")}
              </Link>
            </Box>
          </Box>
          <Box
            className="consumers-list-container"
            widthProp="100%"
            displayProp="flex"
            flexWrap="wrap"
            marginProp="32px 176px 40px 0"
          >
            {consumers.map((consumer, i) => (
              <StyledConsumer
                className="consumer-item-wrapper"
                key={i}
                marginProp="0 24px 24px 0"
              >
                <Separator />
                <Box displayProp="flex" className="consumer-item-container">
                  <ConsumerItem
                    consumer={consumer}
                    consumers={consumers}
                    dialogVisible={dialogVisible}
                    selectedConsumer={selectedConsumer}
                    isLoading={isLoading}
                    onChangeLoading={onChangeLoading}
                    onModalClose={onModalClose}
                    onModalOpen={onModalOpen}
                    onToggleClick={onToggleClick}
                    setConsumer={setConsumer}
                    sendConsumerNewProps={sendConsumerNewProps}
                  />
                </Box>
              </StyledConsumer>
            ))}
          </Box>
        </RootContainer>
        {dialogVisible && (
          <ConsumerModalDialog
            t={t}
            i18n={this.props.i18n}
            dialogVisible={dialogVisible}
            consumers={consumers}
            selectedConsumer={selectedConsumer}
            isLoading={isLoading}
            onModalClose={onModalClose}
            onModalButtonClick={onModalButtonClick}
            onChangeLoading={onChangeLoading}
            sendConsumerNewProps={sendConsumerNewProps}
          />
        )}
      </>
    );
  }
}

const mapStateToProps = (state) => {
  const { consumers } = state.settings.integration;
  return { consumers };
};

export default connect(mapStateToProps, { getConsumers, sendConsumerNewProps })(
  withTranslation()(ThirdPartyServices)
);
