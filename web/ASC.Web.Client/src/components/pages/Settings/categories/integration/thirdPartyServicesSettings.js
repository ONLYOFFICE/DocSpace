import React from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import {
  getConsumers,
  setSelectedConsumer,
  updateConsumerProps,
} from "../../../../../store/settings/actions";
import { getConsumersList } from "../../../../../store/settings/selectors";
import { withTranslation } from "react-i18next";
import styled from "styled-components";

import { Box, Text, Link, toastr } from "asc-web-components";
import { utils } from "asc-web-components";
import { store as commonStore } from "asc-web-common";
import ConsumerItem from "./sub-components/consumerItem";
import ConsumerModalDialog from "./sub-components/consumerModalDialog";

const { getUrlAuthKeys } = commonStore.auth.selectors;

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
    const { t, i18n, consumers, updateConsumerProps, urlAuthKeys } = this.props;
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
                color="#316DAA"
                isHovered={false}
                target="_blank"
                href={urlAuthKeys}
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

const mapStateToProps = (state) => {
  return {
    consumers: getConsumersList(state),
    urlAuthKeys: getUrlAuthKeys(state),
  };
};

export default connect(mapStateToProps, {
  getConsumers,
  updateConsumerProps,
  setSelectedConsumer,
})(withTranslation()(ThirdPartyServices));
