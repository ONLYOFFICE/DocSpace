import React from "react";
import { Box, ToggleButton, Icons } from "asc-web-components";
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

    toggleClick = () => {
        if (this.state.toggleActive) {
            this.setState({
                toggleActive: false
            })
            // TODO: api -> service off -> toastr
        }
        else {
            this.props.onToggleClick();
        }
    }

    render() {

        const { consumer } = this.props;
        const { toggleActive } = this.state;
        const { toggleClick } = this;

        return (
            <>
                <Box>
                    <StyledToggle
                        onChange={toggleClick}
                        isDisabled={!consumer.canSet}
                        isChecked={!consumer.canSet ? true : toggleActive}
                    />
                </Box>
            </>
        );
    }
}

export default ConsumerToggle;