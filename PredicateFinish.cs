using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DiyHomeCache;
using DiyHomeHelpers.B_DiyHomeSDKFunction;
using DiyHomeHelpers.K_Extentions;
using DiyHomeHelpers.O_Product;
using DiyHomeModels.E_ColorModel;
using DiyHomeModels;
using lizzie;

namespace DiyHomeFinish
{
    /// <summary>
    /// 
    /// </summary>
    public class PredicateFinish : Singleton<PredicateFinish>
    {
        int uType = 0;
        string key;

        public void SetKey(string k) => key = k;
        ObjectFinishConfig config;
        public void InitConfig(ObjectFinishConfig conf) => config = conf;


        public PredicateFinish()
        {
            uType =0; //(int)FinishCache.Instance.UserPermission;
        }


        Dictionary<string, PermissionConfigViewModel> dictAllow = FinishCache.Instance.PermissionConfigViewModels_Dict; // #YH021
        string pmCode =UserPermission.DIYHOMEMD.ToString(); // FinishCache.Instance.UserPermission.ToString();


        FinishImageModel o;

        public void InitFinish(FinishImageModel _finish) => o = _finish;


        [Bind(Name = "IsMatchFinishCode")]
        object IsFinishCodeMatch(Binder<PredicateFinish> ctx, Arguments arguments)
        {
            foreach (string i in arguments)
            {
                if (o.FinishCode.StartsWith(i)) return true;
            }
            return null;
        }

        [Bind(Name = "notTargitMatch")]
        object IsPermissionMatch(Binder<PredicateFinish> ctx, Arguments arguments)
        {
            return arguments.Get<int>(0) != this.uType;
        }

        [Bind(Name = "FinishTypeMatch")]
        object IsFinishTypeMatch(Binder<PredicateFinish> ctx, Arguments arguments)
        {
            return arguments.Get<string>(0) == key;
        }

        [Bind(Name = "IsObjCode")]
        object IsObjCodeMatch(Binder<PredicateFinish> ctx, Arguments arguments)
        {
            foreach (string i in arguments)
            {
                if (this.config.Code.StartsWith(i)) return true;
            }
            return false;
        }

        public bool IsMatch(FinishImageModel o)
        {
            string dictKey = o.FinishId.ToString() + pmCode;
            bool bHaveKey = dictAllow.ContainsKey(dictKey);
            if (bHaveKey && dictAllow[dictKey].Status > 0)
            {
                if (uType != 4)//????????????????????????
                {
                    if (key == "KLTS014" && o.FinishCode.Equals("477S"))
                    {
                        return false;
                    }

                    if (o.FinishCode.StringStartsWith("BD217", "BD026", "BD027", "BD907", "477S"))
                    {
                        if (config.Code.StringStartsWith(
                                                        "ZCARHA",//?????????-?????????
                                                        "ZCAGDG",//?????????-??????????????????
                                                        "ZCJBCB"//?????????-?????????????????????
                                                        ))
                        {
                            return false;
                        }
                        if (config.Code.StringStartsWith("MEE") && uType != 2)
                        {
                            return false;
                        }
                    }
                }

                //???????????????
                if (uType != 5) //??????
                {
                    if (key == "001" && config.Code.Equals("ZCAGBA") && o.FinishCode.Equals("907B"))
                    {
                        return false;
                    }
                    if (config.Code.StringStartsWith("ZIPAHB", "ZIPAHA", "ZIPABA", "ZIPABB", "ZIPAIA", "ZIPADA", "ZIPADB",
                                                     "ZIPADC", "ZIPACA", "ZIPACB", "ZIPAKC", "ZIPAKB", "ZIPAKA", "ZIPABC",
                                                     "ZIPAHC", "ZCCAMA", "ZCASAA", "ZCASBA", "ZCASDA", "ZCASDB", "ZCASDC",
                                                     "ZCASDD", "ZBBAKA", "ZBBALC", "ZBBALD", "ZBEFAA")
                                                     && o.FinishCode.StringStartsWith("5F2LC", "6A2LC")) return false;
                }
                //??????????????????C2???C3?????????M190??????
                if (uType != 1 && o.FinishCode.StringStartsWith("M190") && config.Code.StringStartsWith("LAAAA", "LAABA", "LAABB", "LCBAA", "LCBBA", "LCBCA", "LDAAA", "LDAAB", "LFAAA", "LFABA",
                            "LAAAC", "LAAAH", "LAAAI", "LAAAJ", "LAAAK", "LAAAL", "LAABC", "LAABH", "LAABI", "LAABJ", "LAABK", "LAABL", "LFACA", "LFAHA",
                            "LFAHB", "LFAIA", "LFAIB", "LFAJA", "LFAJB", "LFAKA", "LFAKB", "LFALA", "LFALB"))
                {
                    return false;
                }


                if (uType.In(1, 2) && config.Code.StringStartsWith("MBE", "MBD", "MBR", "MBS", "MBT", "MIC", "XMCA"))
                {
                    #region ??????????????????????????????????????????????????????
                    if (key == "013" && !o.FinishCode.StringStartsWith("BD548", "M090", "M290"))
                    {
                        return false;
                    }
                    #endregion
                }
                else if (uType == 2)
                {
                    #region ?????????????????????                                  

                    if (FinishCache.Instance.UnavailableFinish_XQD.ContainsKey(o.FinishCode))
                    {
                        var str = FinishCache.Instance.UnavailableFinish_XQD[o.FinishCode];
                        if (config.Code.StringStartsWith(str.Split(',')))
                        {
                            return false;
                        }
                    }

                    if (FinishCache.Instance.UnavailableFinishByParentCode_XQD.ContainsKey(o.FinishCode))
                    {
                        //????????????????????????????????????????????????
                        var str = FinishCache.Instance.UnavailableFinishByParentCode_XQD[o.FinishCode];
                        if (!string.IsNullOrEmpty(str))
                        {
                            var strlst1 = str.Split(';');
                            foreach (var item in strlst1)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    var strlst2 = item.Split(':');
                                    if (strlst2.Length == 2)
                                    {
                                        string code = strlst2[0];
                                        if (config.Code.StringStartsWith(code))
                                        {
                                            var parenid = DiyHomeSDK.GetRoot(config.UuId);
                                            string pcode = DiyHomeSDK.GetCode(parenid);

                                            if (pcode.StringStartsWith(strlst2[1].Split(',')))
                                            {
                                                return false;
                                            }
                                        }
                                        else if (config.Code.StringStartsWith("ZBGACN", "ZBGACM"))
                                        {
                                            var parenid = DiyHomeSDK.GetRoot(config.UuId);
                                            string pcode = DiyHomeSDK.GetCode(parenid);

                                            if (pcode.StringStartsWith(strlst2[1].Split(',')))
                                            {
                                                var num = DiyHomeSDK.ChildNum(config.UuId);
                                                if (num > 0)
                                                {
                                                    if (DiyHomeSDK.GetCode(DiyHomeSDK.GetChild(config.UuId, 0)).Equals(code))
                                                        return false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (o.FinishCode.Equals("BD027CH") && config.Code.StringStartsWith("ZBGACN", "ZBGACM"))//BD027C_????????????H ???L???????????????????????????
                    {
                        var num = DiyHomeSDK.ChildNum(config.UuId);
                        if (num > 0)
                        {
                            var id = DiyHomeSDK.GetChild(config.UuId, 0);
                            if (DiyHomeSDK.GetCode(id).StringStartsWith("ZBGBDA", "ZBGBDB"))//L??????????????????????????????,L??????????????????????????????
                                return false;
                        }
                        else if (config.Parent != null && config.Parent.UuId > 0 && DiyHomeSDK.GetXData_Text(config.Parent.UuId, "TMLX").Equals("3"))
                            return false;
                    }
                    //??????????????????????????????
                    if (key.StringEqualsWith("004", "YSCBJBJZS") && o.FinishCode.StartsWith("3A3LC"))
                    {
                        return false;
                    }

                    if (config.Code.StartsWith("ZCJBCB") && o.FinishCode.StringStartsWith("BD548", "BD549", "BD560", "BD561", "BD907", "BD543", "BD544"))
                    {
                        return false;
                    }

                    if (config.Code.StartsWith("ZCJBC") && o.FinishCode.StringStartsWith("029", "201"))
                    {
                        return false;
                    }
                    #endregion
                }
                else if (uType == 0)
                {
                    #region ??????????????????
                    if (o.FinishCode.StringStartsWith("915Q") && !config.Code.Equals("ZCAEPA"))
                    {
                        //????????????915Q?????????BB???
                        return false;
                    }
                    else if (key == "013" && config.Code.StringStartsWith("MBE", "MIC", "MBD", "MBR", "MBS", "MBT", "XMCA", "MAEKA", "MAEKB", "MAELA", "MAELB"))
                    {
                        if (o.FinishCode.StringStartsWith("936", "937"))
                        {
                            //?????? ?????????????????? 936,937 ?????????
                            return false;
                        }
                        //?????????????????? ?????????????????????
                        return true;
                    }
                    else if (FinishCache.Instance.UnavailableFinishNoMD.ContainsKey(o.FinishCode))
                    {
                        var str = FinishCache.Instance.UnavailableFinishNoMD[o.FinishCode];
                        if (config.Code.StringStartsWith(str.Split(',')))
                        {
                            return false;
                        }
                    }
                    else if (o.FinishCode.StartsWith("BD479P") && !config.Code.StringStartsWith("IDMBC", "IDMAI", "IDMAG", "IDMAF", "IDMBE", "IDMBF", "IDMCC", "IDMAK", "IDMAJ", "IDMAH", "IDMBH"))
                    {
                        return false;
                    }

                    #endregion
                }
                else if (uType == 4)//?????????????????????
                {
                    if (key == "013" && config.Code.StringStartsWith("MBE", "MIC", "MBD", "MBR", "MBS", "MBT", "XMCA"))
                    {
                        if (o.FinishCode.StringStartsWith("BD548"))
                        {
                            //?????????????????? ???????????????BD548
                            return true;
                        }
                        //?????????????????? ?????????????????????
                        return false;
                    }
                    if (key == "001" && o.FinishCode.StringStartsWith("947", "166", "061"))
                    {
                        if (config.Code.StringStartsWith(FinishCache.Instance.Un947A_166B_061B_MLN))
                        {
                            return false;
                        }
                    }

                    if (config.Code.StartsWith("MEE") && o.FinishCode.StringStartsWith("BD812", "BD813"))
                    {
                        return false;
                    }
                    //?????????????????????????????? TK220241335
                    if (config.Code.StringStartsWith("ZCAGDG", "ZCAGDH") && o.FinishCode.StringStartsWith("BD548", "BD549", "BD560", "BD561", "BD543", "BD544"))
                    {
                        return false;
                    };

                    if (FinishCache.Instance.UnavailableFinishByParentCode_MLN.ContainsKey(o.FinishCode))
                    {
                        //????????????????????????????????????????????????
                        var str = FinishCache.Instance.UnavailableFinishByParentCode_MLN[o.FinishCode];
                        if (!string.IsNullOrEmpty(str))
                        {
                            var strlst1 = str.Split(';');
                            foreach (var item in strlst1)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    var strlst2 = item.Split(':');
                                    if (strlst2.Length == 2)
                                    {
                                        string code = strlst2[0];
                                        if (config.Code.StringStartsWith(code))
                                        {
                                            var parenid = DiyHomeSDK.GetRoot(config.UuId);
                                            string pcode = DiyHomeSDK.GetCode(parenid);

                                            if (pcode.StringStartsWith(strlst2[1].Split(',')))
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (uType == 5)
                {
                    //??????
                    if (key == "013" && config.Code.StringStartsWith("MBE", "MIC", "MBD", "MBR", "MBS", "MBT", "XMCA", "MAEKA", "MAEKB", "MAELA", "MAELB"))
                    {
                        if (o.FinishCode.StringStartsWith("936", "937"))
                        {
                            //?????????????????? 936,937
                            return true;
                        }
                        //?????????????????? ?????????????????????
                        return false;
                    }
                    else if (key == "001" && o.FinishCode.StringStartsWith("220") && config.Code.In("FAACA", "FAACB", "FLCIA", "FLCIB"))
                    {
                        return false;
                    }
                }
                else if (uType == 3)
                {
                    //??????
                    if (key == "013" && config.Code.StringStartsWith("MBE", "MBD", "MBR", "MIC", "MBS", "MBT", "XMCA", "MAEKA", "MAEKB", "MAELA", "MAELB"))
                    {
                        if (o.FinishCode.StringStartsWith("936", "937"))
                        {
                            //?????????????????? 936,937 ??????????????????
                            return false;
                        }
                        //?????????????????? ?????????????????????
                        return true;
                    }
                }
                return true;
            }
            else if (uType == 2)
            {
                if (FinishCache.Instance.OnlyInUseFinish_XQD.ContainsKey(o.FinishCode))
                {
                    if (o.FinishCode.StringStartsWith("908A", "031A") && key != "013")
                    {
                        return false;
                    }

                    var str = FinishCache.Instance.OnlyInUseFinish_XQD[o.FinishCode];
                    if (config.Code.StringStartsWith(str.Split(',')))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

}