import React from "react";
import { Box, ToggleButton, toastr } from "asc-web-components";
import styled from "styled-components";

const StyledToggle = styled(ToggleButton)`
  position: relative;
`;

class ConsumerToggle extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      toggleActive: false,
    };
  }

  onToggleClick = (e) => {
    const { consumer, onModalOpen, sendConsumerNewProps } = this.props;

    if (e.currentTarget.checked) {
      onModalOpen();
    } else {
      this.setState({
        toggleActive: false,
      });

      const prop = [];
      let i = 0;
      let propsLength = Object.keys(consumer.props).length;

      for (i = 0; i < propsLength; i++) {
        prop.push({
          name: consumer.props[i].name,
          value: "",
        });
      }

      const data = {
        name: this.props.consumer.name,
        props: prop,
      };

      sendConsumerNewProps(data)
        .then(() => {
          toastr.info("Consumer successfully deactivated");
        })
        .catch((error) => {
          toastr.error(error);
        });
    }
  };

  render() {
    const { consumer } = this.props;
    const { toggleActive } = this.state;
    const { onToggleClick } = this;

    return (
      <>
        <StyledToggle
          onChange={onToggleClick}
          isDisabled={!consumer.canSet}
          isChecked={
            !consumer.canSet || consumer.props.find((p) => p.value)
              ? true
              : toggleActive
          }
        />
      </>
    );
  }
}

export default ConsumerToggle;
