import React from "react";
import { Box, Text, ToggleButton, Icons } from "asc-web-components";
import styled from "styled-components";

const StyledToggle = styled(ToggleButton)`
   position: relative;
`;

class ConsumerToggle extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            toggleActive: false
        }
    }

    onToggleClick = () => {
        if (this.state.toggleActive) {
            this.setState({
                toggleActive: false
            })
            // TODO: api -> service off -> toastr
        }
        this.props.toggleClick();
        
    }

    render() {

        const { name, onModalOpen, consumer } = this.props;
        const { toggleActive } = this.state;
        const { onToggleClick } = this;

        return (
            <>
                <Box>
                    <StyledToggle
                        onChange={onToggleClick}
                        isDisabled={!consumer.canSet}
                        isChecked={!consumer.canSet ? true : toggleActive}
                    />
                </Box>
            </>
        );
    }
}

export default ConsumerToggle;