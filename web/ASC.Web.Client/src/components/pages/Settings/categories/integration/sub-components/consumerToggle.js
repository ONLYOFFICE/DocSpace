import React from "react";
import { Box, ToggleButton, Icons, toastr } from "asc-web-components";
import styled from "styled-components";
import omit from "lodash/omit";

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

    onToggleClick = (e) => {
        const { toggleActive } = this.state;

        if (e.currentTarget.checked) {
            this.props.onModalOpen();
        }
        else {
            this.setState({
                toggleActive: !toggleActive
            })



            this.props.sendConsumerNewProps()
                .then(() => toastr.info("Settings canceled"))
                .catch((error) => toastr.error(error))
        }
    }
    checkActiveToggle = (e) => {
        let obj;
        obj = (this.props.consumers
            .find((consumer) => consumer.name === e.currentTarget.dataset.toggle))
        obj = omit(obj, ["title", "description", "instruction", "canSet"])
        

        console.log(obj);
        return obj;
    }


    render() {

        const { consumer } = this.props;
        const { toggleActive } = this.state;
        const { onToggleClick } = this;



        return (
            <>
                <Box data-toggle={consumer.name} onClick={this.checkActiveToggle}>
                    <StyledToggle
                        onChange={onToggleClick}
                        isDisabled={!consumer.canSet}
                        isChecked={!consumer.canSet || (consumer.props.find(p => p.value)) ? true : toggleActive}
                    />
                </Box>
            </>
        );
    }
}

export default ConsumerToggle;