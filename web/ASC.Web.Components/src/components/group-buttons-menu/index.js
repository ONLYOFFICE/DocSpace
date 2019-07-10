import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import GroupButton from '../group-button'
import DropDownItem from '../drop-down-item'

const StyledGroupButtonsMenu = styled.div`
    position: sticky;
    top: 0;
    background: #FFFFFF;
    box-shadow: 0px 2px 18px rgba(0, 0, 0, 0.100306);
    height: 56px;
    list-style: none;
    padding: 0 18px 19px 0;
    width: 100%;
    white-space: nowrap;
    display: ${state => state.visible ? 'block' : 'none'};
`;

const CloseButton = styled.div`
    position: absolute;
    right: 20px;
    top: 20px;
    width: 20px;
    height: 20px;

    &:hover{
        cursor: pointer;
    }

    &:before, &:after {
        position: absolute;
        left: 15px;
        content: ' ';
        height: 20px;
        width: 1px;
        background-color: #D8D8D8;
    }

    &:before {
        transform: rotate(45deg);
    }

    &:after {
        transform: rotate(-45deg);
    }
`;

const CheckBox = styled.div`
    display: inline-block;
    margin-left: 20px;
    vertical-align: middle;

    & > * {
        margin: 0px;
    }
`;

class GroupButtonsMenu extends React.Component {

    constructor(props) {
        super(props);    
        this.updateMenu = this.updateMenu.bind(this);    
        this.state = {
          priorityItems: [],
          moreItems: [],
          visible: true
        }
        this.fullMenuArray = this.props.menuItems;
        this.checkBox = this.props.checkBox;
    }
    
    componentWillMount() {
        this.setState({
           priorityItems: this.props.menuItems
        })
    }

    componentDidMount() {
        this.widthsArray = Array.from(this.refs.groupMenu.children).map(item => item.getBoundingClientRect().width);

        window.addEventListener('resize', _.throttle(this.updateMenu), 100);
        this.updateMenu();
    }
    
    howManyItemsInMenuArray(array, outerWidth, initialWidth, minimumNumberInNav) {
        let total = (initialWidth+180);
        for(let i = 0; i < array.length; i++) {
            if(total + array[i] > outerWidth) {
              return i < minimumNumberInNav ? minimumNumberInNav : i;
            } else {
              total += array[i];
            }
        }    
    }

    updateMenu() {
        this.outerWidth = this.refs.groupMenuOuter ? this.refs.groupMenuOuter.getBoundingClientRect().width : 0;
        this.moreMenu = this.refs.moreMenu ? this.refs.moreMenu.getBoundingClientRect().width : 0;

        const arrayAmount = this.howManyItemsInMenuArray(this.widthsArray, this.outerWidth, this.moreMenu, 1);    
        const navItemsCopy = this.fullMenuArray;
        const priorityItems = navItemsCopy.slice(0, arrayAmount);
        
        this.setState({
          priorityItems: priorityItems,
          moreItems: priorityItems.length !== navItemsCopy.length ? navItemsCopy.slice(arrayAmount, navItemsCopy.length) : []
        });
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.updateMenu());
    }

    render() {
        const { priorityItems, moreItems } = this.state;

        const toggle = () => this.setState({visible: !this.props.visible});

        return (
            <StyledGroupButtonsMenu ref="groupMenuOuter" visible={this.state.visible} {...this.state}>
                {this.checkBox && 
                    <CheckBox>{this.checkBox}</CheckBox>
                }
                <div ref="groupMenu" style={{display: 'inline-block'}}>
                {priorityItems.map((item, i) => 
                    <GroupButton key={`navItem-${i}`} 
                                label={item.label} 
                                isDropdown={item.isDropdown} 
                                isSeparator={item.isSeparator}
                                fontWeight={item.fontWeight} 
                                onClick={item.onClick}>
                        {item.children}
                    </GroupButton>
                )}
                </div>
                {moreItems.length > 0 && 
                    <GroupButton ref="moreMenu" isDropdown label={this.props.moreLabel}>
                        {moreItems.map((item, i) => 
                            <DropDownItem 
                                key={`moreNavItem-${i}`} 
                                label={item.label}
                                onClick={item.onClick} />      
                        )}
                    </GroupButton>
                }
                <CloseButton title={this.props.closeTitle} onClick={() => toggle()} />
            </StyledGroupButtonsMenu>
        );
    }
}

export default GroupButtonsMenu;