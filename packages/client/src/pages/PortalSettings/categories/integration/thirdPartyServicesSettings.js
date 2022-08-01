import React from "react";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Box from "@docspace/components/box";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import toastr from "@docspace/components/toast/toastr";
import { tablet, mobile } from "@docspace/components/utils/device";
import { showLoader, hideLoader } from "@docspace/common/utils";
import ConsumerItem from "./sub-components/consumerItem";
import ConsumerModalDialog from "./sub-components/consumerModalDialog";
import { inject, observer } from "mobx-react";
import { Base } from "@docspace/components/themes";

const RootContainer = styled(Box)`
  .title-description-container {
    max-width: 700px;
  }

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
  border: ${(props) => props.theme.client.settings.integration.separatorBorder};
`;

Separator.defaultProps = { theme: Base };

class ThirdPartyServices extends React.Component {
  constructor(props) {
    super(props);
    const { t, setDocumentTitle } = props;

    setDocumentTitle(`${t("ThirdPartyAuthorization")}`);

    this.state = {
      dialogVisible: false,
      isLoading: false,
    };
  }

  componentDidMount() {
    const { getConsumers } = this.props;
    showLoader();
    getConsumers().finally(() => hideLoader());
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
    });
    this.props.setSelectedConsumer();
  };

  setConsumer = (e) => {
    this.props.setSelectedConsumer(e.currentTarget.dataset.consumer);
  };

  updateConsumerValues = (obj, isFill) => {
    isFill && this.onChangeLoading(true);

    const prop = [];
    let i = 0;
    let objLength = Object.keys(isFill ? obj : obj.props).length;

    for (i = 0; i < objLength; i++) {
      prop.push({
        name: isFill ? Object.keys(obj)[i] : obj.props[i].name,
        value: isFill ? Object.values(obj)[i] : "",
      });
    }

    const data = {
      name: isFill ? this.state.selectedConsumer : obj.name,
      props: prop,
    };

    this.props
      .updateConsumerProps(data)
      .then(() => {
        isFill && this.onChangeLoading(false);
        isFill
          ? toastr.success(this.props.t("ThirdPartyPropsActivated"))
          : toastr.success(this.props.t("ThirdPartyPropsDeactivated"));
      })
      .catch((error) => {
        isFill && this.onChangeLoading(false);
        toastr.error(error);
      })

      .finally(isFill && this.onModalClose());
  };

  render() {
    const {
      t,
      i18n,
      consumers,
      updateConsumerProps,
      urlAuthKeys,
      theme,
    } = this.props;
    const { dialogVisible, isLoading } = this.state;
    const { onModalClose, onModalOpen, setConsumer, onChangeLoading } = this;

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
                color={theme.client.settings.integration.linkColor}
                isHovered={false}
                target="_blank"
                href={urlAuthKeys}
              >
                {t("Common:LearnMore")}
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
            {consumers.map((consumer) => (
              <StyledConsumer
                className="consumer-item-wrapper"
                key={consumer.name}
                marginProp="0 24px 24px 0"
              >
                <Separator />
                <Box displayProp="flex" className="consumer-item-container">
                  <ConsumerItem
                    consumer={consumer}
                    dialogVisible={dialogVisible}
                    isLoading={isLoading}
                    onChangeLoading={onChangeLoading}
                    onModalClose={onModalClose}
                    onModalOpen={onModalOpen}
                    setConsumer={setConsumer}
                    updateConsumerProps={updateConsumerProps}
                    t={t}
                  />
                </Box>
              </StyledConsumer>
            ))}
          </Box>
        </RootContainer>
        {dialogVisible && (
          <ConsumerModalDialog
            t={t}
            i18n={i18n}
            dialogVisible={dialogVisible}
            isLoading={isLoading}
            onModalClose={onModalClose}
            onChangeLoading={onChangeLoading}
            updateConsumerProps={updateConsumerProps}
          />
        )}
      </>
    );
  }
}

ThirdPartyServices.propTypes = {
  t: PropTypes.func.isRequired,
  i18n: PropTypes.object.isRequired,
  consumers: PropTypes.arrayOf(PropTypes.object).isRequired,
  urlAuthKeys: PropTypes.string,
  getConsumers: PropTypes.func.isRequired,
  updateConsumerProps: PropTypes.func.isRequired,
  setSelectedConsumer: PropTypes.func.isRequired,
};

export default inject(({ setup, auth }) => {
  const { settingsStore, setDocumentTitle } = auth;
  const { urlAuthKeys, theme } = settingsStore;
  const {
    getConsumers,
    integration,
    updateConsumerProps,
    setSelectedConsumer,
  } = setup;
  const { consumers } = integration;

  return {
    theme,
    consumers,
    urlAuthKeys,
    getConsumers,
    updateConsumerProps,
    setSelectedConsumer,
    setDocumentTitle,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyServices)));
