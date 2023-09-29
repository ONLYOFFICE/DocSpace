import React from "react";
import { ReactSVG } from "react-svg";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import Box from "@docspace/components/box";
import Text from "@docspace/components/text";
import ConsumerToggle from "./consumerToggle";
import { Base } from "@docspace/components/themes";
import { thirdpartiesLogo } from "@docspace/common/utils/image-thirdparties";

const StyledItem = styled.div`
  .consumer-description {
    ${(props) =>
      !props.isThirdPartyAvailable &&
      !props.isSet &&
      css`
        color: #a3a9ae;
      `}
  }
`;

StyledItem.defaultProps = { theme: Base };

const StyledBox = styled(Box)`
  .consumer-icon {
    ${(props) =>
      !props.theme.isBase &&
      css`
        path {
          fill: #ffffff;
          opacity: 1;
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

    ${(props) =>
      !props.isThirdPartyAvailable &&
      props.canSet &&
      css`
        path {
          opacity: 0.5;
        }
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
      isThirdPartyAvailable,
    } = this.props;

    const logo = thirdpartiesLogo?.get(`${consumer.name.toLowerCase()}.svg`);

    const isSet =
      !consumer.canSet || consumer.props.find((p) => p.value) ? true : false;

    return (
      <StyledItem isThirdPartyAvailable={isThirdPartyAvailable} isSet={isSet}>
        <Box
          displayProp="flex"
          justifyContent="space-between"
          alignItems="center"
          widthProp="100%"
        >
          <StyledBox
            canSet={consumer.canSet}
            isLinkedIn={consumer.name === "linkedin"}
            isThirdPartyAvailable={isThirdPartyAvailable}
          >
            {logo && (
              <ReactSVG
                src={logo}
                className={"consumer-icon"}
                alt={consumer.name}
              />
            )}
          </StyledBox>
          <Box onClick={setConsumer} data-consumer={consumer.name}>
            <ConsumerToggle
              consumer={consumer}
              onModalOpen={onModalOpen}
              updateConsumerProps={updateConsumerProps}
              t={t}
              isDisabled={!isThirdPartyAvailable}
            />
          </Box>
        </Box>

        <Text className="consumer-description">{consumer.description}</Text>
      </StyledItem>
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
