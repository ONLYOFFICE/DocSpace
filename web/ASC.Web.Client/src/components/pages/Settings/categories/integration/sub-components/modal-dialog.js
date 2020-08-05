import React from "react";
import { ModalDialog, Text, Button, TextInput } from "asc-web-components";

const ConsumerModalDialog = (props) => {
    const { consumers, name, innerDescription } = props;
    const getConsumerName = (key) => {
        return consumers.find((c,i) => i === key).name;
    }
    return (
        <ModalDialog
            visible={props.dialogVisible}
            headerContent={
                `${name}`
            }
        bodyContent={[<TextInput placeholder={name} />, <Text>{innerDescription}</Text>]}
            footerContent={[<Button primary size="big" label="Send" />]}
            onClose={props.onModalClose}
        />
    )
}

export default ConsumerModalDialog;