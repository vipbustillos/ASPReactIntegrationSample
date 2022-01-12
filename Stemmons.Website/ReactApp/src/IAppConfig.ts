export interface IAppConfig
{
    navWidth: number;
    navCollapsedWidth: number;
    contentDivID: string;
    menuDivID:string;
    leftMenuCollapsed:boolean;
    menu?:{
        top?:IMenuItem[],
        left?:IMenuItem[]
      }
}

export interface IMenuItem
{
    url:string;
    title:string;
    iconName?:string;
    childItems?:IMenuItem[];
    active:boolean;
}