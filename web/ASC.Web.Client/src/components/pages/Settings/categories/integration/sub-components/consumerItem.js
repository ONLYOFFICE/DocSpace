import React from "react";
import { Box, Text, ToggleButton, Icons } from "asc-web-components";
import ConsumerModalDialog from "./modal-dialog";
import styled from "styled-components";

const StyledToggle = styled(ToggleButton)`
   position: relative;
`;

class ConsumerItem extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            toggleActive: false
        }
    }

    onModalButtonClick = () => {
        //TODO: input validate, api request, 
        // refs on toggle?
        this.props.onModalClose();
        this.setState({ toggleActive: true });
        console.log("Click");
    }

    render() {
        const { name, description, dialogVisible, consumers, onModalClose, onToggleClick, selectedConsumer } = this.props;
        const { toggleActive } = this.state;
        const { onModalButtonClick } = this;

        return (
            <>
                <Box displayProp="flex" flexDirection="column" marginProp="16px">
                    <Box displayProp="flex" justifyContent="space-between" widthProp="100%">
                        <div>
                            {React.createElement(Icons[`${name}Icon`], { size: "scale" })}
                        </div>
                        <div>
                            <StyledToggle onChange={onToggleClick} isChecked={toggleActive} />
                        </div>
                    </Box>
                    <Box displayProp="flex" marginProp="10px 10px 10px auto">
                        <Text>
                            {description}
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