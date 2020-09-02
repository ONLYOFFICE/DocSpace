import React from "react";
import { Box, Text, Icons } from "asc-web-components";
import ConsumerToggle from "./consumerToggle";

class ConsumerItem extends React.Component {
    constructor(props) {
        super(props);
        this.state = {}
    }

    render() {

        const { consumer, onToggleClick, setConsumer } = this.props;

        return (
            <>
                <Box displayProp="flex" flexDirection="column">
                    <Box displayProp="flex"
                        justifyContent="space-between"
                        alignItems="center"
                        widthProp="100%"
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
            </>
        );
    }
}

export default ConsumerItem;