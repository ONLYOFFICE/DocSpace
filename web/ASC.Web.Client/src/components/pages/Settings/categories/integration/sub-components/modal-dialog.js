import React from "react";
import { ModalDialog, Text, Button, TextInput, Box, Link } from "asc-web-components";

const ConsumerModalDialog = (props) => {

    const { consumers, selectedConsumer } = props;

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
                <Text>{bodyDescription}</Text>,
                getInputFields()
            ]}
            footerContent={[
                <Button primary size="medium" label="Включить" />,
                <Text>{bottomDescription}</Text>
            ]}
            onClose={props.onModalClose}
        />
    )
}

export default ConsumerModalDialog;