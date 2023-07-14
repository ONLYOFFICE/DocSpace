import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import ToggleButton from "@docspace/components/toggle-button";
import toastr from "@docspace/components/toast/toastr";

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
    const { consumer, onModalOpen, updateConsumerProps, t } = this.props;

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
        name: consumer.name,
        props: prop,
      };

      updateConsumerProps(data)
        .then(() => {
          toastr.success(t("ThirdPartyPropsDeactivated"));
        })
        .catch((error) => {
          toastr.error(error);
        });
    }
  };

  render() {
    const { consumer, isDisabled } = this.props;
    const { toggleActive } = this.state;
    const { onToggleClick } = this;

    return (
      <>
        <StyledToggle
          className={consumer.name}
          onChange={onToggleClick}
          isDisabled={!consumer.canSet || isDisabled}
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

ConsumerToggle.propTypes = {
  consumer: PropTypes.shape({
    name: PropTypes.string,
    title: PropTypes.string,
    description: PropTypes.string,
    instruction: PropTypes.string,
    canSet: PropTypes.bool,
    props: PropTypes.arrayOf(PropTypes.object),
  }).isRequired,
  onModalOpen: PropTypes.func.isRequired,
  updateConsumerProps: PropTypes.func.isRequired,
};
