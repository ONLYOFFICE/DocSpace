import React, { Component } from "react";
import PropTypes from "prop-types";
import StyledChart from "./styled-chart";
import {
  Chart as ChartJs,
  ArcElement,
  LineElement,
  BarElement,
  PointElement,
  BarController,
  BubbleController,
  DoughnutController,
  LineController,
  PieController,
  PolarAreaController,
  RadarController,
  ScatterController,
  CategoryScale,
  LinearScale,
  LogarithmicScale,
  RadialLinearScale,
  TimeScale,
  TimeSeriesScale,
  Decimation,
  Filler,
  Legend,
  Title,
  Tooltip,
} from "chart.js";

ChartJs.register(
  ArcElement,
  LineElement,
  BarElement,
  PointElement,
  BarController,
  BubbleController,
  DoughnutController,
  LineController,
  PieController,
  PolarAreaController,
  RadarController,
  ScatterController,
  CategoryScale,
  LinearScale,
  LogarithmicScale,
  RadialLinearScale,
  TimeScale,
  TimeSeriesScale,
  Decimation,
  Filler,
  Legend,
  Title,
  Tooltip
);

class Chart extends Component {
  initChart() {
    this.chart = new ChartJs(this.canvas, {
      type: this.props.type,
      data: this.props.data,
      options: this.props.options,
    });

    console.log(this.chart);
  }

  getCanvas() {
    return this.canvas;
  }

  getBase64Image() {
    return this.chart.toBase64Image();
  }

  generateLegend() {
    if (this.chart) {
      this.chart.generateLegend();
    }
  }

  refresh() {
    if (this.chart) {
      this.chart.update();
    }
  }

  reinit() {
    if (this.chart) {
      this.chart.destroy();
    }
    this.initChart();
  }

  shouldComponentUpdate(nextProps) {
    if (
      nextProps.data === this.props.data &&
      nextProps.options === this.props.options &&
      nextProps.type === this.props.type
    ) {
      return false;
    }

    return true;
  }

  componentDidMount() {
    this.initChart();
  }

  componentDidUpdate() {
    this.reinit();
  }

  componentWillUnmount() {
    if (this.chart) {
      this.chart.destroy();
      this.chart = null;
    }
  }

  render() {
    const { width, height, className, id, style } = this.props;

    let commonStyle = Object.assign(
      {
        width: width,
        height: height,
      },
      style
    );

    return (
      <StyledChart id={id} style={commonStyle} className={className}>
        <canvas
          ref={(el) => {
            this.canvas = el;
          }}
          width={width}
          height={height}
        ></canvas>
      </StyledChart>
    );
  }
}

Chart.propTypes = {
  /** Used as HTML id property */
  id: PropTypes.string,
  /** Type of the chart */
  type: PropTypes.string,
  /** Data to display */
  data: PropTypes.object,
  /** Options to customize the chart */
  options: PropTypes.object,
  /** Width of the chart in non-responsive mode */
  width: PropTypes.string,
  /** Height of the chart in non-responsive mode */
  height: PropTypes.string,
  /** Inline style of the element */
  style: PropTypes.object,
  /** Style class of the element */
  className: PropTypes.string,
};

Chart.defaultProps = {
  id: null,
  type: null,
  data: null,
  options: null,
  width: null,
  height: null,
  style: null,
  className: null,
};

export default Chart;
