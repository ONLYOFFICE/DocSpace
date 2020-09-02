import React from "react";
import { ModalDialog, Text, Button, TextInput, Box, Link } from "asc-web-components";

class ConsumerModalDialog extends React.Component {

    constructor(props) {
        super(props);
        this.state = {};
    }

    mapTokenNameToState = () => {
        const { consumers, selectedConsumer } = this.props;
        consumers
            .find((consumer) => consumer.name === selectedConsumer).props
            .map(p => this.setState(
                {
                    [`${p.name}`]: p.value
                }
            ))
    }

    onChangeHandler = e => {
        this.setState({
            [e.target.name]: e.target.value
        })
    }

    onSendValues = () => {
        this.props.onModalButtonClick();
        
        const prop = [];
        let i = 0;
        let stateLength = Object.keys(this.state).length;
        for (i = 0; i < stateLength; i++) {
            prop.push({
                name: Object.keys(this.state)[i],
                value: Object.values(this.state)[i]
            })
        }

        console.log([{
            name: this.props.selectedConsumer,
            props: prop
        }]);
    }

    componentDidMount() {
        this.mapTokenNameToState()
    }

    render() {

        const { consumers, selectedConsumer, onModalClose, dialogVisible } = this.props;
        const { onChangeHandler } = this;

        const bodyDescription = (
            <>
                <Text isBold={true}>Как это работает?</Text>
                <Text>Для получения подробных инструкций по подключению этого сервиса, пожалуйста, перейдите в наш Справочный центр, где приводится вся необходимая информация.</Text>
            </>
        );
        const bottomDescription = (
            <>
                <Text>
                    Если у вас остались вопросы по подключению этого сервиса или вам требуется помощь, вы всегда можете обратиться в нашу
                {" "}
                    <Link
                        isHovered={true}
                        target="_blank"
                        href="http://support.onlyoffice.com/ru">
                        Службу техподдержки
                            </Link>.</Text></>
        );

        const getConsumerName = () => {
            return consumers.find((consumer) => consumer.name === selectedConsumer).name;
        }
        const getInnerDescription = () => {
            return consumers.find((consumer) => consumer.name === selectedConsumer).instruction;
        }
        const getInputFields = () => {

            return consumers
                .find((consumer) => consumer.name === selectedConsumer)
                .props
                .map((prop, i) =>
                    <React.Fragment key={i}>
                        <Box displayProp="flex" flexDirection="column">
                            <Box>
                                <Text isBold={true}>{prop.title}:</Text>
                            </Box>
                            <Box>
                                <TextInput
                                    name={prop.name}
                                    placeholder={prop.title}
                                    isAutoFocussed={i === 0 && true}
                                    value={Object.values(this.state)[i]}
                                    onChange={onChangeHandler}
                                />
                            </Box>
                        </Box>
                    </React.Fragment>
                )
        }

        return (
            <ModalDialog
                visible={dialogVisible}
                headerContent={`${getConsumerName()}`}
                bodyContent={[
                    <Text>{getInnerDescription()}</Text>,
                    <Text>{bodyDescription}</Text>,
                    <React.Fragment>{getInputFields()}</React.Fragment>
                ]}
                footerContent={[
                    <Button
                        primary
                        size="medium"
                        label="Включить"
                        onClick={this.onSendValues} />,
                    <Text>{bottomDescription}</Text>
                ]}
                onClose={onModalClose}
            />
        )
    }
}

export default ConsumerModalDialog;