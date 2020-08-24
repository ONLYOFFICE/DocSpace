import React from "react";
import { Box, Text, ToggleButton, Icons } from "asc-web-components";
import styled from "styled-components";

const StyledToggle = styled(ToggleButton)`
   position: relative;
`;

class ConsumerItemToggle extends React.Component {

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

        // this.setState({
        //     toggleActive: true
        // });
        else {
            this.props.onToggleClick();
        }

    }



    render() {

        const { name, onModalOpen } = this.props;
        const { toggleActive } = this.state;
        const { onToggleClick } = this;

        return (
            <>
                <div>
                    <StyledToggle onChange={onToggleClick}
                        isChecked={toggleActive} />
                </div>
            </>
        );
    }
}

export default ConsumerItemToggle;