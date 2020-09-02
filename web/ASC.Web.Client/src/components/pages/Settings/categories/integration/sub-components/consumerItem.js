import React from "react";
import { Box, Text, ToggleButton, Icons } from "asc-web-components";
import ConsumerModalDialog from "./consumerModalDialog";
import ConsumerToggle from "./consumerToggle";

class ConsumerItem extends React.Component {
    constructor(props) {
        super(props);
        this.state = {}
    }

    // onButtonClick = () => {
    //     //TODO: api -> set tokens, 
    //     this.props.onModalClose();
    //     //this.setState({ toggleActive: true });
    //     console.log(this.props.selectedConsumer);
    // }

    render() {

        const { dialogVisible, consumer, consumers, onModalClose, selectedConsumer, onToggleClick, setConsumer } = this.props;
        const { onButtonClick } = this;

        return (
            <>
                <Box displayProp="flex" flexDirection="column">
                    <Box displayProp="flex"
                        justifyContent="space-between"
                        alignItems="center"
                        widthProp="100%"
                        //marginProp="21px 0 0 0"
                    >
                        <Box>
                            <Text>{consumer.name}</Text>
                            {/* {React.createElement(Icons[`${name}Icon`], { size: "scale" })} */}
                        </Box>
                        <Box onClick={setConsumer} data-consumer={consumer.name} marginProp="28px 0 0 0">
                            <ConsumerToggle
                                consumer={consumer}
                                onToggleClick={onToggleClick}
                            />
                        </Box>

                    </Box>
                    <Box displayProp="flex" marginProp="21px 0 0 0">
                        <Text>
                            {consumer.instruction}
                        </Text>
                    </Box>
                </Box>
                {/* {dialogVisible &&
                    <ConsumerModalDialog
                        dialogVisible={dialogVisible}
                        onModalClose={onModalClose}
                        consumers={consumers}
                        consumer={consumer}
                        selectedConsumer={selectedConsumer}
                        onButtonClick={onButtonClick}
                    />} */}
            </>
        );
    }
}

export default ConsumerItem;