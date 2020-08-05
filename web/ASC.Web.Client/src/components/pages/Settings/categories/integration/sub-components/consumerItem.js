import React from "react";
import { Box, Text } from "asc-web-components";
import ConsumerModalDialog from "./modal-dialog";

const ConsumerItem = (props) => {

    const { name, description, dialogVisible, consumers, onModalClose, onToggleClick, selectedConsumer } = props;

    return (
        <>
            <Box displayProp="flex" flexDirection="column" marginProp="16px">
                <Box displayProp="flex" justifyContent="space-between" widthProp="100%">
                    <Box displayProp="flex">
                        <Text>
                            {name} logo
                        </Text>
                    </Box>
                    <Box onClick={onToggleClick}>
                        toggle
                        </Box>
                </Box>
                <Box displayProp="flex" marginProp="10px 10px 10px auto">
                    <Text>
                        {description}
                    </Text>
                </Box>
            </Box>
            {dialogVisible && <ConsumerModalDialog
                dialogVisible={dialogVisible}
                onModalClose={onModalClose}
                consumers={consumers}
                selectedConsumer={selectedConsumer}
            />}
        </>
    )
}

export default ConsumerItem;