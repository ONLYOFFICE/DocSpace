import React from "react";
import { Box, Text, Icons } from "asc-web-components";
import ConsumerToggle from "./consumerToggle";

class ConsumerItem extends React.Component {
    constructor(props) {
        super(props);
        this.state = {}
    }

    render() {

        const { consumer, consumers, onModalOpen, setConsumer, sendConsumerNewProps } = this.props;

        return (
            <>
                <Box displayProp="flex" flexDirection="column">
                    <Box displayProp="flex"
                        justifyContent="space-between"
                        alignItems="center"
                        widthProp="100%"
                        heightProp="56px"
                    >
                        <Box>
                            {React.createElement(Icons[`${consumer.name}Icon`], { size: "scale" })}
                        </Box>
                        <Box onClick={setConsumer} data-consumer={consumer.name}>
                            <ConsumerToggle
                                consumers={consumers}
                                consumer={consumer}
                                onModalOpen={onModalOpen}
                                sendConsumerNewProps={sendConsumerNewProps}
                            />
                        </Box>
                    </Box>
                    <Box displayProp="flex" marginProp="21px 0 0 0">
                        <Text>
                            {consumer.description}
                        </Text>
                    </Box>
                </Box>
            </>
        );
    }
}

export default ConsumerItem;