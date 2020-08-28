import React from "react";
import { Box, Text, ToggleButton, Icons } from "asc-web-components";
import ConsumerModalDialog from "./consumerModalDialog";

class ConsumerItem extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            //toggleActive: false
        }
    }

    onModalButtonClick = () => {
        //TODO: input validate, api -> set tokens, 
        this.props.onModalClose();
        //this.setState({ toggleActive: true });
        console.log(this.props.selectedConsumer);
    }

    render() {
        const { dialogVisible, consumer, consumers, onModalClose, selectedConsumer } = this.props;
        const { toggleActive } = this.state;
        const { onModalButtonClick } = this;

        return (
            <>
                <Box displayProp="flex" flexDirection="column">
                    <Box displayProp="flex" justifyContent="space-between" widthProp="100%" marginProp="21px 0 0 0">
                        <div>
                            <Text>{consumer.name}</Text>
                            {/* {React.createElement(Icons[`${name}Icon`], { size: "scale" })} */}
                        </div>
                    </Box>
                    <Box displayProp="flex" marginProp="21px 0 0 0">
                        <Text>
                            {consumer.instruction}
                        </Text>
                    </Box>
                </Box>
                {dialogVisible &&
                    <ConsumerModalDialog
                        dialogVisible={dialogVisible}
                        onModalClose={onModalClose}
                        consumers={consumers}
                        selectedConsumer={selectedConsumer}
                        onModalButtonClick={onModalButtonClick}
                        toggleActive={toggleActive}
                    />}
            </>
        );
    }
}

export default ConsumerItem;