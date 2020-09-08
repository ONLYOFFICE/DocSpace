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

class ConsumerModalDialog extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }

    mapTokenNameToState = () => {
        const { consumers, selectedConsumer } = this.props;
        consumers
            .find((consumer) => consumer.name === selectedConsumer)
            .props.map((p) =>
                this.setState({
                    [`${p.name}`]: p.value,
                })
            );
    };

    onChangeHandler = (e) => {
        this.setState({
            [e.target.name]: e.target.value,
        });
    };

    updateConsumerValues = () => {
        this.props.onChangeLoading(true);

        const prop = [];

        let i = 0;
        let stateLength = Object.keys(this.state).length;
        for (i = 0; i < stateLength; i++) {
            prop.push({
                name: Object.keys(this.state)[i],
                value: Object.values(this.state)[i],
            });
        }
        const data = {
            name: this.props.selectedConsumer,
            props: prop,
        };
        this.props
            .sendConsumerNewProps(data)
            .then(() => {
                this.props.onChangeLoading(false);
                toastr.success("Consumer properties successfully update");
            })
            .catch((error) => {
                this.props.onChangeLoading(false);
                toastr.error(error);
            })
            .finally(this.props.onModalClose());
    };

    componentDidMount() {
        this.mapTokenNameToState();
    }

    render() {
        const {
            consumers,
            selectedConsumer,
            onModalClose,
            dialogVisible,
            isLoading,
            t,
            i18n,
        } = this.props;
        const { onChangeHandler } = this;

        const bodyDescription = (
            <Box marginProp="44px 0 16px 0">
                <Box marginProp="0 0 16px 0">
                    <Text isBold={true} fontSize="15px">
                        {t("ThirdPartyHowItWorks")}
                    </Text>
                </Box>
                <Text>{t("ThirdPartyBodyDescription")}</Text>
            </Box>
        );

        const bottomDescription = (
            <Trans i18nKey="ThirdPartyBottomDescription" i18n={i18n}>
                If you still have some questions on how to connect this service or need
        technical assistance, please feel free to contact our{" "}
                <Link
                    color="#316DAA"
                    isHovered={false}
                    target="_blank"
                    href={"http://support.onlyoffice.com/"}
                >
                    Support Team
        </Link>
            </Trans>
        );

        const getConsumerName = () => {
            return consumers.find((consumer) => consumer.name === selectedConsumer)
                .name;
        };
        const getInnerDescription = () => {
            return consumers.find((consumer) => consumer.name === selectedConsumer)
                .instruction;
        };
        const getInputFields = () => {
            return consumers
                .find((consumer) => consumer.name === selectedConsumer)
                .props.map((prop, i) => (
                    <React.Fragment key={i}>
                        <Box
                            displayProp="flex"
                            flexDirection="column"
                            marginProp="0 0 16px 0"
                        >
                            <Box marginProp="0 0 4px 0">
                                <Text isBold={true}>{prop.title}:</Text>
                            </Box>
                            <Box>
                                <TextInput
                                    scale
                                    name={prop.name}
                                    placeholder={prop.title}
                                    isAutoFocussed={i === 0 && true}
                                    tabIndex={1}
                                    value={Object.values(this.state)[i]}
                                    isDisabled={isLoading}
                                    onChange={onChangeHandler}
                                />
                            </Box>
                        </Box>
                    </React.Fragment>
                ));
        };

        return (
            <ModalDialogContainer>
                <ModalDialog
                    visible={dialogVisible}
                    headerContent={`${getConsumerName()}`}
                    bodyContent={[
                        <Text>{getInnerDescription()}</Text>,
                        <Text>{bodyDescription}</Text>,
                        <React.Fragment>{getInputFields()}</React.Fragment>,
                        <Text>{bottomDescription}</Text>,
                    ]}
                    footerContent={[
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
                            onClick={this.updateConsumerValues}
                        />,
                    ]}
                    onClose={onModalClose}
                />
            </ModalDialogContainer>
        );
    }
}

export default ConsumerModalDialog;

ConsumerModalDialog.propTypes = {
    t: PropTypes.func.isRequired,
    i18n: PropTypes.object.isRequired,
    consumers: PropTypes.array.isRequired,
    selectedConsumer: PropTypes.string,
    onModalClose: PropTypes.func.isRequired,
    dialogVisible: PropTypes.bool.isRequired,
    isLoading: PropTypes.bool.isRequired,
    onChangeLoading: PropTypes.func.isRequired,
    sendConsumerNewProps: PropTypes.func.isRequired
}
