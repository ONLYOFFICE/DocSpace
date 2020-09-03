import React from "react";
import { connect } from "react-redux";
import { getConsumers } from '../../../../../store/settings/actions';
import { withTranslation } from 'react-i18next';
import styled from "styled-components";

import { Box, Text, Link } from "asc-web-components";
import ConsumerItem from "./sub-components/consumerItem";
import ConsumerModalDialog from "./sub-components/consumerModalDialog";

const RootContainer = styled(Box)`

  @media (max-width: 768px) {
    margin: 0;

    .consumers-list-container {
        margin: 32px 0 40px 0;
        }
    }
`;
const StyledConsumer = styled(Box)`
  width: 400px;

  @media (max-width: 768px) {
    width: 496px;
  }

  @media (max-width: 375px) {
    margin: 0;
  }
`;
const Separator = styled.div`
 border: 1px solid #ECEEF1;
`;

class ThirdPartyServices extends React.Component {

    constructor(props) {
        super(props);
        const { t } = props;
        document.title = `${t("ThirdPartyAuthorization")} – ${t("OrganizationName")}`;

        this.state = {
            selectedConsumer: "",
            dialogVisible: false
        }
    }

    componentDidMount() {
        const { getConsumers } = this.props;
        getConsumers();
    }

    onModalOpen = () => {
        this.setState({
            dialogVisible: true
        })
    }

    onModalClose = () => {
        this.setState({
            dialogVisible: false,
            selectedConsumer: ""
        })
    }

    onModalButtonClick = () => {
        //TODO: api -> set tokens, 
        //this.onModalClose();
        //this.setState({ toggleActive: true });
        console.log(this.state.selectedConsumer);
    }

    onToggleClick = () => {
        this.onModalOpen();
    }

    setConsumer = (e) => {
        this.setState({
            selectedConsumer: e.currentTarget.dataset.consumer
        })
    }

    render() {

        const { t, consumers } = this.props;
        const { selectedConsumer, dialogVisible } = this.state;
        const { onModalClose, onToggleClick, setConsumer, onModalButtonClick } = this;

        return (
            <>
                <RootContainer displayProp="flex" flexDirection="column" marginProp="0 88px 0 0">
                    <Box className="title-description-container">
                        <Text>
                            {t("ThirdPartyTitleDescription")}
                        </Text>
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
                        {consumers
                            .map((consumer, i) =>
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
                                            onModalClose={onModalClose}
                                            onToggleClick={onToggleClick}
                                            setConsumer={setConsumer}
                                        />
                                    </Box>
                                </StyledConsumer>
                            )}
                    </Box>
                </RootContainer>
                {dialogVisible &&
                    <ConsumerModalDialog
                        t={t}
                        i18n={this.props.i18n}
                        dialogVisible={dialogVisible}
                        consumers={consumers}
                        selectedConsumer={selectedConsumer}
                        onModalClose={onModalClose}
                        onModalButtonClick={onModalButtonClick}
                    />}
            </>
        )
    }
}

const mapStateToProps = (state) => {
    const { consumers } = state.settings.integration;
    return { consumers }
}

export default connect(mapStateToProps, { getConsumers })(withTranslation()(ThirdPartyServices));