import styled from "styled-components";

import { Base } from "@docspace/components/themes";

const StyledErrorContainer = styled.div`
  background: ${(props) => props.theme.errorContainer.background};
  cursor: default;
  width: 100%;
  overflow-x: hidden;
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 0;
  padding-top: 36px;
  border: 0;
  box-sizing: border-box;

  #container {
    position: relative;
    margin: 12px 0 60px 0;
  }

  #header {
    font-weight: 600;
    font-size: 30px;
    line-height: 41px;
    margin: 0 0 24px 0;
    text-align: center;
  }

  #text {
    font-size: 16px;
    line-height: 22px;
    text-align: center;
    margin: 0 0 24px 0;
    max-width: 560px;
    padding: 0;
  }

  #button-container {
    margin: 0 0 auto 0;
  }

  #button {
    display: inline-block;
    margin: 0 0 36px 0;
  }

  @media screen and (max-width: 960px) {
    body {
      padding: 24px 24px 0 24px;
    }

    #container {
      margin: 12px 0 48px 0;
    }

    #button {
      margin: 0 0 24px 0;
    }
  }

  @media screen and (max-width: 620px) {
    body {
      padding: 18px 18px 0 18px;
    }

    #container {
      margin: 12px 0 36px 0;
    }

    #header {
      font-size: 18px;
      line-height: 25px;
    }

    #text {
      font-size: 13px;
      line-height: 18px;
    }

    #button-container {
      align-self: stretch;
      margin: auto 0 0 0;
    }

    #button {
      width: 100%;
      margin: 0 0 18px 0;
    }
  }

  #background {
    width: 100%;
    height: auto;
    -webkit-animation: fadein_background 1s;
    -moz-animation: fadein_background 1s;
    -ms-animation: fadein_background 1s;
    -o-animation: fadein_background 1s;
    animation: fadein_background 1s;
  }

  @keyframes fadein_background {
    from {
      opacity: 0;
    }

    to {
      opacity: 1;
    }
  }

  @-moz-keyframes fadein_background {
    from {
      opacity: 0;
    }

    to {
      opacity: 1;
    }
  }

  @-webkit-keyframes fadein_background {
    from {
      opacity: 0;
    }

    to {
      opacity: 1;
    }
  }

  @-ms-keyframes fadein_background {
    from {
      opacity: 0;
    }

    to {
      opacity: 1;
    }
  }

  #birds {
    position: absolute;
    left: 56.8%;
    top: 27.4%;
    width: 35%;
    height: 33.7%;
    z-index: 1;
    -webkit-animation: fadein_birds 1s;
    -moz-animation: fadein_birds 1s;
    -ms-animation: fadein_birds 1s;
    -o-animation: fadein_birds 1s;
    animation: fadein_birds 1s;
  }

  @keyframes fadein_birds {
    from {
      opacity: 0;
      left: 56.8%;
      top: 0;
    }

    to {
      opacity: 1;
      left: 56.8%;
      top: 27.4%;
    }
  }

  @-moz-keyframes fadein_birds {
    from {
      opacity: 0;
      left: 56.8%;
      top: 0;
    }

    to {
      opacity: 1;
      left: 56.8%;
      top: 27.4%;
    }
  }

  @-webkit-keyframes fadein_birds {
    from {
      opacity: 0;
      left: 56.8%;
      top: 0;
    }

    to {
      opacity: 1;
      left: 56.8%;
      top: 27.4%;
    }
  }

  @-ms-keyframes fadein_birds {
    from {
      opacity: 0;
      left: 56.8%;
      top: 0;
    }

    to {
      opacity: 1;
      left: 56.8%;
      top: 27.4%;
    }
  }

  #mountain-left {
    position: absolute;
    left: 10.66%;
    top: 63.01%;
    width: 25.46%;
    height: 35.61%;
    z-index: 2;
    -webkit-animation: fadein_mountain-left 1s;
    -moz-animation: fadein_mountain-left 1s;
    -ms-animation: fadein_mountain-left 1s;
    -o-animation: fadein_mountain-left 1s;
    animation: fadein_mountain-left 1s;
  }

  @keyframes fadein_mountain-left {
    from {
      opacity: 0;
      left: 10.66%;
      top: 90.4%;
    }

    to {
      opacity: 1;
      left: 10.66%;
      top: 63.01%;
    }
  }

  @-moz-keyframes fadein_mountain-left {
    from {
      opacity: 0;
      left: 10.66%;
      top: 90.4%;
    }

    to {
      opacity: 1;
      left: 10.66%;
      top: 63.01%;
    }
  }

  @-webkit-keyframes fadein_mountain-left {
    from {
      opacity: 0;
      left: 10.66%;
      top: 90.4%;
    }

    to {
      opacity: 1;
      left: 10.66%;
      top: 63.01%;
    }
  }

  @-ms-keyframes fadein_mountain-left {
    from {
      opacity: 0;
      left: 10.66%;
      top: 90.4%;
    }

    to {
      opacity: 1;
      left: 10.66%;
      top: 63.01%;
    }
  }

  #mountain-right {
    position: absolute;
    left: 58.66%;
    top: 54.79%;
    width: 30.66%;
    height: 44.38%;
    z-index: 3;
    -webkit-animation: fadein_mountain-right 1s;
    -moz-animation: fadein_mountain-right 1s;
    -ms-animation: fadein_mountain-right 1s;
    -o-animation: fadein_mountain-right 1s;
    animation: fadein_mountain-right 1s;
  }

  @keyframes fadein_mountain-right {
    from {
      opacity: 0;
      left: 58.66%;
      top: 82.19%;
    }

    to {
      opacity: 1;
      left: 58.66%;
      top: 54.79%;
    }
  }

  @-moz-keyframes fadein_mountain-right {
    from {
      opacity: 0;
      left: 58.66%;
      top: 82.19%;
    }

    to {
      opacity: 1;
      left: 58.66%;
      top: 54.79%;
    }
  }

  @-webkit-keyframes fadein_mountain-right {
    from {
      opacity: 0;
      left: 58.66%;
      top: 82.19%;
    }

    to {
      opacity: 1;
      left: 58.66%;
      top: 54.79%;
    }
  }

  @-ms-keyframes fadein_mountain-right {
    from {
      opacity: 0;
      left: 58.66%;
      top: 82.19%;
    }

    to {
      opacity: 1;
      left: 58.66%;
      top: 54.79%;
    }
  }

  #mountain-center {
    position: absolute;
    left: 24.8%;
    top: 45.47%;
    width: 48.53%;
    height: 66.3%;
    z-index: 5;
    -webkit-animation: fadein_mountain-center 1s;
    -moz-animation: fadein_mountain-center 1s;
    -ms-animation: fadein_mountain-center 1s;
    -o-animation: fadein_mountain-center 1s;
    animation: fadein_mountain-center 1s;
  }

  @keyframes fadein_mountain-center {
    from {
      opacity: 0;
      left: 24.8%;
      top: 72.87%;
    }

    to {
      opacity: 1;
      left: 24.8%;
      top: 45.47%;
    }
  }

  @-moz-keyframes fadein_mountain-center {
    from {
      opacity: 0;
      left: 24.8%;
      top: 72.87%;
    }

    to {
      opacity: 1;
      left: 24.8%;
      top: 45.47%;
    }
  }

  @-webkit-keyframes fadein_mountain-center {
    from {
      opacity: 0;
      left: 24.8%;
      top: 72.87%;
    }

    to {
      opacity: 1;
      left: 24.8%;
      top: 45.47%;
    }
  }

  @-ms-keyframes fadein_mountain-center {
    from {
      opacity: 0;
      left: 24.8%;
      top: 72.87%;
    }

    to {
      opacity: 1;
      left: 24.8%;
      top: 45.47%;
    }
  }

  #white-cloud-behind {
    position: absolute;
    left: 57.33%;
    top: 63.01%;
    width: 8.4%;
    height: 7.39%;
    z-index: 4;
    -webkit-animation: fadein_white-cloud-behind 1s ease-in,
      move_white-cloud-behind 1s linear 1s infinite alternate;
    -moz-animation: fadein_white-cloud-behind 1s ease-in,
      move_white-cloud-behind 1s linear 1s infinite alternate;
    -ms-animation: fadein_white-cloud-behind 1s ease-in,
      move_white-cloud-behind 1s linear 1s infinite alternate;
    -o-animation: fadein_white-cloud-behind 1s ease-in,
      move_white-cloud-behind 1s linear 1s infinite alternate;
    animation: fadein_white-cloud-behind 1s ease-in,
      move_white-cloud-behind 1s linear 1s infinite alternate;
  }

  @keyframes fadein_white-cloud-behind {
    from {
      opacity: 0;
      left: 57.33%;
      top: 90.41%;
    }

    to {
      opacity: 1;
      left: 57.33%;
      top: 63.01%;
    }
  }

  @-moz-keyframes fadein_white-cloud-behind {
    from {
      opacity: 0;
      left: 57.33%;
      top: 90.41%;
    }

    to {
      opacity: 1;
      left: 57.33%;
      top: 63.01%;
    }
  }

  @-webkit-keyframes fadein_white-cloud-behind {
    from {
      opacity: 0;
      left: 57.33%;
      top: 90.41%;
    }

    to {
      opacity: 1;
      left: 57.33%;
      top: 63.01%;
    }
  }

  @-ms-keyframes fadein_white-cloud-behind {
    from {
      opacity: 0;
      left: 57.33%;
      top: 90.41%;
    }

    to {
      opacity: 1;
      left: 57.33%;
      top: 63.01%;
    }
  }

  @keyframes move_white-cloud-behind {
    from {
      top: 63.01%;
    }

    to {
      top: 64.65%;
    }
  }

  @-moz-keyframes move_white-cloud-behind {
    from {
      top: 63.01%;
    }

    to {
      top: 64.65%;
    }
  }

  @-webkit-keyframes move_white-cloud-behind {
    from {
      top: 63.01%;
    }

    to {
      top: 64.65%;
    }
  }

  @-ms-keyframes move_white-cloud-behind {
    from {
      top: 63.01%;
    }

    to {
      top: 64.65%;
    }
  }

  #white-cloud-center {
    position: absolute;
    left: 31.33%;
    top: 73.97%;
    width: 9.86%;
    height: 9.04%;
    z-index: 6;
    -webkit-animation: fadein_white-cloud-center 1s ease-in,
      move_white-cloud-center 1s linear 1s infinite alternate;
    -moz-animation: fadein_white-cloud-center 1s ease-in,
      move_white-cloud-center 1s linear 1s infinite alternate;
    -ms-animation: fadein_white-cloud-center 1s ease-in,
      move_white-cloud-center 1s linear 1s infinite alternate;
    -o-animation: fadein_white-cloud-center 1s ease-in,
      move_white-cloud-center 1s linear 1s infinite alternate;
    animation: fadein_white-cloud-center 1s ease-in,
      move_white-cloud-center 1s linear 1s infinite alternate;
  }

  @keyframes fadein_white-cloud-center {
    from {
      opacity: 0;
      left: 31.33%;
      top: 101.36%;
    }

    to {
      opacity: 1;
      left: 31.33%;
      top: 73.97%;
    }
  }

  @-moz-keyframes fadein_white-cloud-center {
    from {
      opacity: 0;
      left: 31.33%;
      top: 101.36%;
    }

    to {
      opacity: 1;
      left: 31.33%;
      top: 73.97%;
    }
  }

  @-webkit-keyframes fadein_white-cloud-center {
    from {
      opacity: 0;
      left: 31.33%;
      top: 101.36%;
    }

    to {
      opacity: 1;
      left: 31.33%;
      top: 73.97%;
    }
  }

  @-ms-keyframes fadein_white-cloud-center {
    from {
      opacity: 0;
      left: 31.33%;
      top: 101.36%;
    }

    to {
      opacity: 1;
      left: 31.33%;
      top: 73.97%;
    }
  }

  @keyframes move_white-cloud-center {
    from {
      top: 73.97%;
    }

    to {
      top: 72.32%;
    }
  }

  @-moz-keyframes move_white-cloud-center {
    from {
      top: 73.97%;
    }

    to {
      top: 72.32%;
    }
  }

  @-webkit-keyframes move_white-cloud-center {
    from {
      top: 73.97%;
    }

    to {
      top: 72.32%;
    }
  }

  @-ms-keyframes move_white-cloud-center {
    from {
      top: 73.97%;
    }

    to {
      top: 72.32%;
    }
  }

  #white-cloud-left {
    position: absolute;
    left: -0.66%;
    top: 80.82%;
    width: 24%;
    height: 21.91%;
    z-index: 7;
    -webkit-animation: fadein_white-cloud-left 1s ease-in;
    -moz-animation: fadein_white-cloud-left 1s ease-in;
    -ms-animation: fadein_white-cloud-left 1s ease-in;
    -o-animation: fadein_white-cloud-left 1s ease-in;
    animation: fadein_white-cloud-left 1s ease-in;
  }

  @keyframes fadein_white-cloud-left {
    from {
      opacity: 0;
      left: -14%;
      top: 80.82%;
    }

    to {
      opacity: 1;
      left: -0.66%;
      top: 80.82%;
    }
  }

  @-moz-keyframes fadein_white-cloud-left {
    from {
      opacity: 0;
      left: -14%;
      top: 80.82%;
    }

    to {
      opacity: 1;
      left: -0.66%;
      top: 80.82%;
    }
  }

  @-webkit-keyframes fadein_white-cloud-left {
    from {
      opacity: 0;
      left: -14%;
      top: 80.82%;
    }

    to {
      opacity: 1;
      left: -0.66%;
      top: 80.82%;
    }
  }

  @-ms-keyframes fadein_white-cloud-left {
    from {
      opacity: 0;
      left: -14%;
      top: 80.82%;
    }

    to {
      opacity: 1;
      left: -0.66%;
      top: 80.82%;
    }
  }

  #white-cloud-right {
    position: absolute;
    left: 81.33%;
    top: 86.3%;
    width: 21.33%;
    height: 19.17%;
    z-index: 8;
    -webkit-animation: fadein_white-cloud-right 1s ease-in;
    -moz-animation: fadein_white-cloud-right 1s ease-in;
    -ms-animation: fadein_white-cloud-right 1s ease-in;
    -o-animation: fadein_white-cloud-right 1s ease-in;
    animation: fadein_white-cloud-right 1s ease-in;
  }

  @keyframes fadein_white-cloud-right {
    from {
      opacity: 0;
      left: 94.66%;
      top: 86.3%;
    }

    to {
      opacity: 1;
      left: 81.33%;
      top: 86.3%;
    }
  }

  @-moz-keyframes fadein_white-cloud-right {
    from {
      opacity: 0;
      left: 94.66%;
      top: 86.3%;
    }

    to {
      opacity: 1;
      left: 81.33%;
      top: 86.3%;
    }
  }

  @-webkit-keyframes fadein_white-cloud-right {
    from {
      opacity: 0;
      left: 94.66%;
      top: 86.3%;
    }

    to {
      opacity: 1;
      left: 81.33%;
      top: 86.3%;
    }
  }

  @-ms-keyframes fadein_white-cloud-right {
    from {
      opacity: 0;
      left: 94.66%;
      top: 86.3%;
    }

    to {
      opacity: 1;
      left: 81.33%;
      top: 86.3%;
    }
  }

  #blue-cloud-left {
    position: absolute;
    left: 0;
    top: 43.83%;
    width: 8.4%;
    height: 6.57%;
    z-index: 9;
    -webkit-animation: fadein_blue-cloud-left 1s ease-in;
    -moz-animation: fadein_blue-cloud-left 1s ease-in;
    -ms-animation: fadein_blue-cloud-left 1s ease-in;
    -o-animation: fadein_blue-cloud-left 1s ease-in;
    animation: fadein_blue-cloud-left 1s ease-in;
  }

  @keyframes fadein_blue-cloud-left {
    from {
      opacity: 0;
      left: -13.33%;
      top: 43.83%;
    }

    to {
      opacity: 1;
      left: 0;
      top: 43.83%;
    }
  }

  @-moz-keyframes fadein_blue-cloud-left {
    from {
      opacity: 0;
      left: -13.33%;
      top: 43.83%;
    }

    to {
      opacity: 1;
      left: 0;
      top: 43.83%;
    }
  }

  @-webkit-keyframes fadein_blue-cloud-left {
    from {
      opacity: 0;
      left: -13.33%;
      top: 43.83%;
    }

    to {
      opacity: 1;
      left: 0;
      top: 43.83%;
    }
  }

  @-ms-keyframes fadein_blue-cloud-left {
    from {
      opacity: 0;
      left: -13.33%;
      top: 43.83%;
    }

    to {
      opacity: 1;
      left: 0;
      top: 43.83%;
    }
  }

  #blue-cloud-right {
    position: absolute;
    left: 87.33%;
    top: 24.65%;
    width: 11.33%;
    height: 9.04%;
    z-index: 1;
    -webkit-animation: fadein_blue-cloud-right 1s ease-in;
    -moz-animation: fadein_blue-cloud-right 1s ease-in;
    -ms-animation: fadein_blue-cloud-right 1s ease-in;
    -o-animation: fadein_blue-cloud-right 1s ease-in;
    animation: fadein_blue-cloud-right 1s ease-in;
  }

  @keyframes fadein_blue-cloud-right {
    from {
      opacity: 0;
      left: 100.66%;
      top: 24.65%;
    }

    to {
      opacity: 1;
      left: 87.33%;
      top: 24.65%;
    }
  }

  @-moz-keyframes fadein_blue-cloud-right {
    from {
      opacity: 0;
      left: 100.66%;
      top: 24.65%;
    }

    to {
      opacity: 1;
      left: 87.33%;
      top: 24.65%;
    }
  }

  @-webkit-keyframes fadein_blue-cloud-right {
    from {
      opacity: 0;
      left: 100.66%;
      top: 24.65%;
    }

    to {
      opacity: 1;
      left: 87.33%;
      top: 24.65%;
    }
  }

  @-ms-keyframes fadein_blue-cloud-right {
    from {
      opacity: 0;
      left: 100.66%;
      top: 24.65%;
    }

    to {
      opacity: 1;
      left: 87.33%;
      top: 24.65%;
    }
  }

  #baloon {
    position: absolute;
    left: 25.33%;
    top: 13.69%;
    width: 12.26%;
    height: 38.08%;
    z-index: 11;
    -webkit-animation: fadein_baloon 1s,
      move_baloon 1s linear 1s infinite alternate;
    -moz-animation: fadein_baloon 1s,
      move_baloon 1s linear 1s infinite alternate;
    -ms-animation: fadein_baloon 1s, move_baloon 1s linear 1s infinite alternate;
    -o-animation: fadein_baloon 1s, move_baloon 1s linear 1s infinite alternate;
    animation: fadein_baloon 1s, move_baloon 1s linear 1s infinite alternate;
  }

  @keyframes fadein_baloon {
    from {
      left: 25.33%;
      top: 8.21%;
    }

    to {
      left: 25.33%;
      top: 13.69%;
    }
  }

  @-moz-keyframes fadein_baloon {
    from {
      left: 25.33%;
      top: 8.21%;
    }

    to {
      left: 25.33%;
      top: 13.69%;
    }
  }

  @-webkit-keyframes fadein_baloon {
    from {
      left: 25.33%;
      top: 8.21%;
    }

    to {
      left: 25.33%;
      top: 13.69%;
    }
  }

  @-ms-keyframes fadein_baloon {
    from {
      left: 25.33%;
      top: 8.21%;
    }

    to {
      left: 25.33%;
      top: 13.69%;
    }
  }

  @keyframes move_baloon {
    from {
      top: 13.69%;
    }

    to {
      top: 15.34%;
    }
  }

  @-moz-keyframes move_baloon {
    from {
      top: 13.69%;
    }

    to {
      top: 15.34%;
    }
  }

  @-webkit-keyframes move_baloon {
    from {
      top: 13.69%;
    }

    to {
      top: 15.34%;
    }
  }

  @-ms-keyframes move_baloon {
    from {
      top: 13.69%;
    }

    to {
      top: 15.34%;
    }
  }
`;

StyledErrorContainer.defaultProps = { theme: Base };

export default StyledErrorContainer;
