import React from "react";
import { ReactSVG } from "react-svg";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import Box from "@docspace/components/box";
import Text from "@docspace/components/text";
import ConsumerToggle from "./consumerToggle";
import { Base } from "@docspace/components/themes";

const StyledBox = styled(Box)`
  .consumer-icon {
    ${(props) =>
      !props.theme.isBase &&
      css`
        path {
          fill: #ffffff;
          opacity: ${props.isSet ? 1 : 0.16};
        }
        ${props.isLinkedIn &&
        css`
          path:nth-child(8) {
            fill: #333333;
            opacity: 1;
          }
          path:nth-child(9) {
            fill: #333333;
            opacity: 1;
          }
        `}
      `}
  }
`;

StyledBox.defaultProps = { theme: Base };

class ConsumerItem extends React.Component {
  render() {
    const {
      consumer,
      onModalOpen,
      setConsumer,
      updateConsumerProps,
      t,
    } = this.props;

    const logo = `/images/thirdparties/${consumer.name.toLowerCase()}.svg`;

    return (
      <>
        <Box displayProp="flex" flexDirection="column" widthProp="100%">
          <Box
            displayProp="flex"
            justifyContent="space-between"
            alignItems="center"
            widthProp="100%"
            heightProp="56px"
          >
            <StyledBox
              isSet={
                !consumer.canSet || consumer.props.find((p) => p.value)
                  ? true
                  : false
              }
              isLinkedIn={consumer.name === "linkedin"}
            >
              <ReactSVG
                src={logo}
                className={"consumer-icon"}
                alt={consumer.name}
              />
            </StyledBox>
            <Box onClick={setConsumer} data-consumer={consumer.name}>
              <ConsumerToggle
                consumer={consumer}
                onModalOpen={onModalOpen}
                updateConsumerProps={updateConsumerProps}
                t={t}
              />
            </Box>
          </Box>
          <Box displayProp="flex" marginProp="21px 0 0 0">
            <Text>{consumer.description}</Text>
          </Box>
        </Box>
      </>
    );
  }
}

export default ConsumerItem;

ConsumerItem.propTypes = {
  consumer: PropTypes.shape({
    name: PropTypes.string,
    title: PropTypes.string,
    description: PropTypes.string,
    instruction: PropTypes.string,
    canSet: PropTypes.bool,
    props: PropTypes.arrayOf(PropTypes.object),
  }).isRequired,
  onModalOpen: PropTypes.func.isRequired,
  setConsumer: PropTypes.func.isRequired,
  updateConsumerProps: PropTypes.func.isRequired,
};
