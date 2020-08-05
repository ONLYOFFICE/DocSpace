import React from "react";
import { ModalDialog, Text, Button, TextInput, Box } from "asc-web-components";

const ConsumerModalDialog = (props) => {

    const { consumers, selectedConsumer } = props;

    const getConsumerName = () => {
        return consumers.find((consumer) => consumer.name === selectedConsumer).name;
    }
    const getInnerDescription = () => {
        return consumers.find((consumer) => consumer.name === selectedConsumer).innerDescription;
    }
    const getInputFields = () => {
        return consumers
            .find((consumer) => consumer.name === selectedConsumer)
            .tokens
            .map((token) =>
                <>
                    <Box displayProp="flex" flexDirection="column">
                        <Box>
                            <Text isBold={true}>{token}:</Text>
                        </Box>
                        <Box>
                            <TextInput placeholder={token} />
                        </Box>
                    </Box>
                </>
            )
    }

    return (
        <ModalDialog
            visible={props.dialogVisible}
            headerContent={`${getConsumerName()}`}
            bodyContent={[
                <Text>{getInnerDescription()}</Text>,
                getInputFields()
            ]}
            footerContent={[
                <Button primary size="medium" label="Включить" />
            ]}
            onClose={props.onModalClose}
        />
    )
}

export default ConsumerModalDialog;