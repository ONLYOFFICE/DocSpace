import React, { Component } from 'react'
import { findDOMNode } from 'react-dom'
import screenfull from 'screenfull'

import ReactPlayer from 'react-player'
import Duration from './duration'
import styled from "styled-components"
import { Icons } from "asc-web-components";


const StyledControls = styled.div`
    height: 40px;
    display: block;
    position: absolute;
    z-index: 4001;
    top: calc(50% + 113px);
    left: calc(50% - 200px);
    
`;
const StyledVideoControlBtn = styled.div`
    display: inline-block;
    height: 30px;
    line-height: 30px;
    margin: 5px;
    width: 40px;
    border-radius: 2px;
    cursor: pointer;
    text-align: center;
    &:hover{
        background-color: rgba(200,200,200,0.2);
    }
`;

const VideoControlBtn = props => {
    return (
        <StyledVideoControlBtn {...props} >
            {props.children}
        </StyledVideoControlBtn>
    );
}
const Controls = props => {
  return (
    <StyledControls {...props} >
        {props.children}
    </StyledControls>
  );
}

class PlayBtn extends Component{
  constructor(props) {
      super(props);
  }
  render(){
      return(
          <VideoControlBtn  onClick={this.props.onClick}>
              {this.props.playing ? <Icons.ItemPausedIcon size="medium" isfill={true} color="#fff" /> : <Icons.ItemActiveIcon size="medium" isfill={true} color="#fff" />}
          </VideoControlBtn>
      );
  }
}
class FullScreenBtn extends Component{
  constructor(props) {
      super(props);
  }
  render(){
      return(
          <VideoControlBtn  onClick={this.props.onClick}>
              <Icons.ExternalLinkIcon size="medium" isfill={true} color="#fff" />
          </VideoControlBtn>
      );
  }
}

const StyledProgress = styled.div`
  display: inline-block;

  .slider-container{
    display:inline-block;
    border-radius: 2px;
    position:relative;	
    width: ${props => props.width}px;
    height:6px;
    background:rgba(200,200,200,0.2);
    margin:15px 0;
  }
  .fill{
    width: ${props => 100 * props.value}%;
    position:absolute;
    
    top:calc(50% - 3px);
    height:6px;
    background: #d1d1d1;
    border-radius: 2px;
    
  }
  input[type='range'] {
      display: block;
      overflow: visible;
      background: transparent;
      width: ${props => props.width}px;
      height:6px;
      outline:none;
      margin:0;
      -webkit-appearance: none;
      position:relative;
      
  }

  input[type='range']::-webkit-slider-thumb {
      position:relative;
      appearance: none;
	    box-sizing: content-box;
      width: 12px;
      height: 12px;
      margin-top: -3px;
      background: white;
      border-radius: 50%;
      
      cursor: pointer;
      
  }
  input[type=range]::-moz-range-thumb {
      position:relative;
      appearance: none;
	    box-sizing: content-box;
      width: 12px;
      height: 12px;
      background: white;
      border-radius: 50%;
      margin-top: -3px;
      cursor: pointer;
     
  }
  input[type=range]::-ms-thumb {
      position:relative;
      appearance: none;
	    box-sizing: content-box;
      width: 12px;
      height: 12px;
      background: white;
      border-radius: 50%;
      margin-top: -3px;
      cursor: pointer;
      
  }

  input[type='range']::-webkit-slider-runnable-track {
    margin: 12px 0;
    height: 6px;
    border-radius: 2px;
    cursor: pointer;
    -webkit-appearance: none;
    text-align: right;
    pointer-events: none;
    
  }
  input[type="range"]::-moz-range-track  {
    margin: 12px 0;
    height: 6px;
    border-radius: 2px;
    cursor: pointer;
    -webkit-appearance: none;
    text-align: right;
    pointer-events: none;
    
  }
  input[type=range]::-ms-track { 
    border-color: transparent;
    color: transparent;
    
    margin: 12px 0;
    height: 6px;
    border-radius: 2px;
    cursor: pointer;
    -webkit-appearance: none;
    text-align: right;
    pointer-events: none;
    
  }
`;

const Progress = props => {
  return (
    <StyledProgress {...props} >
      <div className="slider-container">
        <div className="fill"></div>
        <input
          type='range' min={0} max={0.999999} step='any'
          value={props.value}
          onMouseDown={props.handleSeekMouseDown}
          onChange={props.handleSeekChange}
          onMouseUp={props.handleSeekMouseUp}
        />
      </div>
    </StyledProgress>
  );
}

const StyledValumeContainer = styled.div`
    display: inline-block;
    position: relative;
    
    .muteConteiner{
      display: none;
      position: absolute;
      width: 40px;
      height: 80px;
      border-radius: 5px;
      top: -76px;
      left: 5px;
      background: black;
    }
    &:hover{
      .muteConteiner{
        display: inline-block;
      }
    }
    .mute{
      display: inline-block;
      transform: rotate(-90deg);
      float: right;
      margin: 22px -12px;
    }
`;
const StyledDuration = styled.div`
    display: inline-block;
    height: 30px;
    line-height: 30px;
    margin: 5px;
    width: 60px;
    text-align: center;
    border-radius: 2px;
    cursor: pointer;

    &:hover{
      background-color: rgba(200,200,200,0.2);
    }
`;
const StyledVideoViewer = styled.div`

    color: #d1d1d1;

    .playerWrapper{
      width: 400px;
      height: 226px;
      left: calc(50% - 200px);
      top: calc(50% - 113px);
      z-index: 4001;
      position: absolute;
      padding-bottom: 40px;
      background-color: rgba(11,11,11,0.7);
    }
`;

class ValumeBtn extends Component{
  constructor(props) {
    super(props);
  }

  render(){
      return(
        <StyledValumeContainer>
          <div className="muteConteiner">
            <Progress
                className="mute"
                width = {this.props.width}
                value = {this.props.volume}
                onMouseDown={this.props.onMouseDown}
                onChange={this.props.onChange}
                onMouseUp={this.props.handleSeekMouseUp}
              />
          </div>
          
          <VideoControlBtn  onClick={this.props.onChangeMute}>
              {this.props.muted ? <Icons.ToggleButtonIcon size="medium" /> : <Icons.ToggleButtonCheckedIcon size="medium" />}
          </VideoControlBtn>
        </StyledValumeContainer>
      );
  }
}

 

const MULTIPLE_SOURCES = [
  { src: 'http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4', type: 'video/mp4' },
  { src: 'http://clips.vorwaerts-gmbh.de/big_buck_bunny.ogv', type: 'video/ogv' },
  { src: 'http://clips.vorwaerts-gmbh.de/big_buck_bunny.webm', type: 'video/webm' }
]

class VideoViewer extends Component {
  state = {
    url: this.props.url,
    pip: false,
    playing: true,
    controls: false,
    light: false,
    volume: 0.8,
    muted: false,
    played: 0,
    loaded: 0,
    duration: 0,
    playbackRate: 1.0,
    loop: false
  }

  load = url => {
    this.setState({
      url,
      played: 0,
      loaded: 0,
      pip: false
    })
  }

  handlePlayPause = () => {
      console.log('handlePlayPause')
    this.setState({ playing: !this.state.playing })
  }

  handleStop = () => {
    this.setState({ url: null, playing: false })
  }

  handleToggleControls = () => {
    const url = this.state.url
    this.setState({
      controls: !this.state.controls,
      url: null
    }, () => this.load(url))
  }

  handleToggleLight = () => {
    this.setState({ light: !this.state.light })
  }

  handleToggleLoop = () => {
    this.setState({ loop: !this.state.loop })
  }

  handleVolumeChange = e => {
    this.setState({ 
      volume: parseFloat(e.target.value),
      muted: false
    })
  }

  handleToggleMuted = () => {
    this.setState({ muted: !this.state.muted })
  }

  handleSetPlaybackRate = e => {
    this.setState({ playbackRate: parseFloat(e.target.value) })
  }
 
  handleTogglePIP = () => {
    this.setState({ pip: !this.state.pip })
  }

  handlePlay = () => {
    console.log('onPlay')
    this.setState({ playing: true })
  }

  handleEnablePIP = () => {
    console.log('onEnablePIP')
    this.setState({ pip: true })
  }

  handleDisablePIP = () => {
    console.log('onDisablePIP')
    this.setState({ pip: false })
  }

  handlePause = () => {
    console.log('onPause')
    this.setState({ playing: false })
  }

  handleSeekMouseDown = e => {
    this.setState({ seeking: true })
  }

  handleSeekChange = e => {
    this.setState({ played: parseFloat(e.target.value) })
  }

  handleSeekMouseUp = e => {
    this.setState({ seeking: false })
    this.player.seekTo(parseFloat(e.target.value))
  }

  handleProgress = state => {
    console.log('onProgress', state)
    // We only want to update time slider if we are not currently seeking
    if (!this.state.seeking) {
      this.setState(state)
    }
  }

  handleEnded = () => {
    console.log('onEnded')
    this.setState({ playing: this.state.loop })
  }

  handleDuration = (duration) => {
    console.log('onDuration', duration)
    this.setState({ duration })
  }

  handleClickFullscreen = () => {
    screenfull.request(findDOMNode(this.player))
  }

  renderLoadButton = (url, label) => {
    return (
      <button onClick={() => this.load(url)}>
        {label}
      </button>
    )
  }

  ref = player => {
    this.player = player
  }

  render () {
    const { url, playing, controls, light, volume, muted, loop, played, loaded, duration, playbackRate, pip } = this.state
    const SEPARATOR = ' · '

    return (
        <StyledVideoViewer>
          <div>
            <div className='playerWrapper'>
              <ReactPlayer
                ref={this.ref}
                className='react-player'
                width='100%'
                height='100%'
                url={url}
                pip={pip}
                playing={playing}
                controls={controls}
                light={light}
                loop={loop}
                playbackRate={playbackRate}
                volume={volume}
                muted={muted}
                onReady={() => console.log('onReady')}
                onStart={() => console.log('onStart')}
                onPlay={this.handlePlay}
                onEnablePIP={this.handleEnablePIP}
                onDisablePIP={this.handleDisablePIP}
                onPause={this.handlePause}
                onBuffer={() => console.log('onBuffer')}
                onSeek={e => console.log('onSeek', e)}
                onEnded={this.handleEnded}
                onError={e => console.log('onError', e)}
                onProgress={this.handleProgress}
                onDuration={this.handleDuration}
              />
            </div>
            <Controls>
              <PlayBtn onClick={this.handlePlayPause} playing={playing} />
              <Progress
                value={played}
                width={180}
                onMouseDown={this.handleSeekMouseDown}
                onChange={this.handleSeekChange}
                onMouseUp={this.handleSeekMouseUp}
              />
              <StyledDuration>-<Duration seconds={duration * (1 - played)} /></StyledDuration>
              <ValumeBtn
                width={64}
                muted={muted}
                volume={muted ? 0 : volume}
                onChangeMute={this.handleToggleMuted}
                onChange={this.handleVolumeChange}
              />

              <FullScreenBtn onClick={this.handleClickFullscreen} />
            </Controls>
          </div>
        </StyledVideoViewer>
    )
  }
}


export default VideoViewer;