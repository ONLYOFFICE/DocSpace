import React from "react";
import { consumers } from "./sub-components/consumers";
import { Box, Text, Link } from "asc-web-components";
import ConsumerItem from "./sub-components/consumerItem";
import { withTranslation } from 'react-i18next';
import { connect } from "react-redux";
import ConsumerItemToggle from "./sub-components/consumerItemToggle";
import styled from "styled-components";

const RootContainer = styled(Box)`

  @media (max-width: 768px) {
    margin: 0;
    }
`;
const ConsumersContainer = styled(Box)`

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
            consumers: consumers,
            selectedConsumer: "",
            dialogVisible: false
        }
    }

    onModalOpen = () => {
        this.setState({
            dialogVisible: true,
        })
    }

    onModalClose = () => {
        this.setState({
            dialogVisible: false,
            selectedConsumer: ""
        })
    }

    titleDescription = "Ключи авторизации позволяют подключить портал ONLYOFFICE к сторонним сервисам, таким как Twitter, Facebook, Dropbox и т.д. Подключите портал к Facebook, Twitter или Linkedin, если Вы не хотите каждый раз при входе вводить свои учетные данные на портале. Привяжите портал к таким сервисам, как Dropbox, OneDrive и т.д. чтобы перенести документы из всех этих хранилищ в модуль Документы ONLYOFFICE."

    render() {

        const { t } = this.props;
        const { consumers, selectedConsumer, dialogVisible } = this.state;
        const { titleDescription, onModalOpen, onModalClose } = this;

        const consumerRefs = consumers.reduce((acc, consumer) => {
            acc[consumer.name] = React.createRef();
            return acc;
        }, []);

        const toggleRefs = consumers.reduce((acc, consumer) => {
            acc[consumer.name] = React.createRef();
            return acc;
        }, []);

        return (
            <>
                <RootContainer displayProp="flex" flexDirection="column" marginProp="0 88px 0 0">
                    <Box className="title-description-container">
                        <Text>
                            {titleDescription}
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
                        alignItems="stretch"
                        alignContent="stretch"
                        marginProp="32px 176px 40px 0"
                    >
                        {consumers
                            .map((consumer, i) =>
                                <ConsumersContainer key={i} widthProp="400px" marginProp="0 24px 24px 0">
                                    <Separator />
                                    <Box displayProp="flex" className="consumer-item-container">
                                        <ConsumerItem
                                            ref={el => (consumerRefs[i] = el)}
                                            name={consumer.name}
                                            description={consumer.description}
                                            consumers={consumers}
                                            dialogVisible={dialogVisible}
                                            selectedConsumer={selectedConsumer}
                                            onModalClose={onModalClose}
                                        />
                                        <ConsumerItemToggle
                                            ref={el => (toggleRefs[i] = el)}
                                            name={consumer.name}
                                            onToggleClick={() => {
                                                this.setState({ selectedConsumer: consumer.name })
                                                onModalOpen()
                                            }}
                                        />
                                    </Box>
                                </ConsumersContainer>
                            )}
                    </Box>
                </RootContainer>
            </>
        )
    }
}

export default connect(null, null)(withTranslation()(ThirdPartyServices));